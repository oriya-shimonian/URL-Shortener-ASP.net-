using UrlShortener.Api.Infrastructure;
using UrlShortener.Api.Services;
using FluentValidation;
using UrlShortener.Api.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ShortUrlService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddValidatorsFromAssemblyContaining<CreateShortUrlRequestValidator>();

var app = builder.Build();

// Always bind to the container PORT (Render/compose/etc.)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

// ✅ Enable Swagger when running in container OR in Development
var runningInContainer = string.Equals(
    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
    "true",
    StringComparison.OrdinalIgnoreCase);

if (app.Environment.IsDevelopment() || runningInContainer)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// במצב קונטיינר/דמו עדיף בלי HTTPS redirection (ה-HTTPS יגיע מהפרוקסי/Render)
//// app.UseHttpsRedirection();

app.MapControllers();

// ✅ Health endpoint so "/" won't 404
app.MapGet("/", () => Results.Ok(new { status = "OK", env = app.Environment.EnvironmentName }));

app.Run();
