using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Application.DTOs;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortUrlsController : ControllerBase
{
    private readonly ShortUrlService _service;

    public ShortUrlsController(ShortUrlService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShortUrlRequest request)
    {
        var result = await _service.CreateShortUrlAsync(request);
        return Created(result.ShortUrl, result);
    }

    [HttpGet("/go/{code}")]
    public async Task<IActionResult> RedirectToOriginal(string code)
    {
        var shortUrl = await _service.GetByCodeAsync(code);
        if (shortUrl == null)
            return NotFound(new { message = "Short code not found", code });

        await _service.RecordClickAsync(
            shortUrl,
            Request.Headers.Referer.ToString(),
            Request.Headers.UserAgent.ToString(),
            HttpContext.Connection.RemoteIpAddress?.ToString()
        );

        return Redirect(shortUrl.OriginalUrl);
    }

    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetStats(int id)
    {
        var stats = await _service.GetStatsAsync(id);
        if (stats == null) return NotFound();
        return Ok(stats);
    }
}


/* 
// ------------------ API שלנו ------------------

// יצירת לינק מקוצר
app.MapPost("/api/short-urls", async (AppDbContext db, HttpRequest req, string originalUrl) =>
{
    string code;
    do
    {
        code = CodeGenerator.GenerateCode();
    } while (await db.ShortUrls.AnyAsync(s => s.Code == code));

    var entity = new ShortUrl { Code = code, OriginalUrl = originalUrl };
    db.ShortUrls.Add(entity);
    await db.SaveChangesAsync();

    // בונים URL מלא (לחיץ בסוואגר)
    var shortUrl = $"{req.Scheme}://{req.Host}/{code}";
    var openUrl  = $"{req.Scheme}://{req.Host}/go/{code}";
    var preview  = $"{req.Scheme}://{req.Host}/api/short-urls/preview/{code}";

    // עדיף גם Location עם כתובת מלאה
    return Results.Created(shortUrl, new
    {
        id = entity.Id,
        code = entity.Code,
        originalUrl = entity.OriginalUrl,
        shortUrl,
        openUrl,
        preview
    });
});

// === GET: הקיצור האמיתי (302). ב-Swagger תראי CORS וזה תקין; בשורת הכתובת עובד ===
app.MapGet("/{code}", async (AppDbContext db, HttpContext ctx, HttpRequest req, string code) =>
{
    var s = await db.ShortUrls.FirstOrDefaultAsync(x => x.Code == code);
    if (s is null) return Results.NotFound(new { message = "Short code not found", code });

    db.Clicks.Add(new Click {
        ShortUrlId = s.Id,
        Referrer   = req.Headers.Referer.ToString(),
        UserAgent  = req.Headers.UserAgent.ToString(),
        IpHash     = ctx.Connection.RemoteIpAddress?.ToString()
    });
    await db.SaveChangesAsync();

    if (Uri.TryCreate(s.OriginalUrl, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        return Results.Redirect(s.OriginalUrl);

    return Results.Ok(new { note = "Invalid original URL; cannot redirect.", s.OriginalUrl });
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
        RecentClicks = shortUrl.Clicks.OrderByDescending(c => c.Timestamp).Take(10).Select(c => new
        {
            c.Timestamp,
            c.Referrer,
            c.UserAgent
        })
    };

    return Results.Ok(stats);
});

// ---------------------------------------------

*/