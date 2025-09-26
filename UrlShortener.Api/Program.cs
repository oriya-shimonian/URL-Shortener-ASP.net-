using UrlShortener.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Domain;
using UrlShortener.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok("OK"));

// ------------------ API שלנו ------------------

// יצירת לינק מקוצר
app.MapPost("/api/short-urls", async (AppDbContext db, string originalUrl) =>
{
    // מייצרים קוד חדש
    string code;
    do
    {
        code = CodeGenerator.GenerateCode();
    } while (await db.ShortUrls.AnyAsync(s => s.Code == code));

    var shortUrl = new ShortUrl
    {
        Code = code,
        OriginalUrl = originalUrl
    };

    db.ShortUrls.Add(shortUrl);
    await db.SaveChangesAsync();

    return Results.Created($"/{code}", new { shortUrl.Id, shortUrl.Code, shortUrl.OriginalUrl });
});

// Redirect + רישום קליק
app.MapGet("/{code}", async (AppDbContext db, HttpContext ctx, string code) =>
{
    var shortUrl = await db.ShortUrls.FirstOrDefaultAsync(s => s.Code == code);
    if (shortUrl is null)
        return Results.NotFound();

    // לוג קליק
    var click = new Click
    {
        ShortUrlId = shortUrl.Id,
        Referrer = ctx.Request.Headers.Referer.ToString(),
        UserAgent = ctx.Request.Headers.UserAgent.ToString(),
        IpHash = ctx.Connection.RemoteIpAddress?.ToString() // אפשר לשפר ל-Hash אמיתי
    };

    db.Clicks.Add(click);
    await db.SaveChangesAsync();

    return Results.Redirect(shortUrl.OriginalUrl);
});

// סטטיסטיקות
app.MapGet("/api/short-urls/{id}/stats", async (AppDbContext db, int id) =>
{
    var shortUrl = await db.ShortUrls.Include(s => s.Clicks).FirstOrDefaultAsync(s => s.Id == id);
    if (shortUrl is null)
        return Results.NotFound();

    var stats = new
    {
        shortUrl.Id,
        shortUrl.Code,
        shortUrl.OriginalUrl,
        ClickCount = shortUrl.Clicks.Count,
        RecentClicks = shortUrl.Clicks.OrderByDescending(c => c.Timestamp).Take(10)            .Select(c => new {
                c.Timestamp,
                c.Referrer,
                c.UserAgent
            })
    };

    return Results.Ok(stats);
});

// ---------------------------------------------

app.Run();
