using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Application.DTOs;
using UrlShortener.Api.Data;
using UrlShortener.Api.Domain;

namespace UrlShortener.Api.Services;

public class ShortUrlService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContext;

    public ShortUrlService(AppDbContext db, IHttpContextAccessor httpContext)
    {
        _db = db;
        _httpContext = httpContext;
    }

    public async Task<ShortUrlResponse> CreateShortUrlAsync(CreateShortUrlRequest request)
    {
        string code;
        do { code = CodeGenerator.GenerateCode(); }
        while (await _db.ShortUrls.AnyAsync(s => s.Code == code));

        var entity = new ShortUrl { Code = code, OriginalUrl = request.OriginalUrl };
        _db.ShortUrls.Add(entity);
        await _db.SaveChangesAsync();

        var req = _httpContext.HttpContext!.Request;
        return new ShortUrlResponse
        {
            Id = entity.Id,
            Code = entity.Code,
            OriginalUrl = entity.OriginalUrl,
            ShortUrl = $"{req.Scheme}://{req.Host}/{code}",
            OpenUrl  = $"{req.Scheme}://{req.Host}/go/{code}",
            Preview  = $"{req.Scheme}://{req.Host}/api/shorturls/{entity.Id}/stats"
        };
    }

    public async Task<ShortUrl?> GetByCodeAsync(string code)
        => await _db.ShortUrls.FirstOrDefaultAsync(s => s.Code == code);

    public async Task RecordClickAsync(ShortUrl shortUrl, string? referrer, string? userAgent, string? ip)
    {
        _db.Clicks.Add(new Click
        {
            ShortUrlId = shortUrl.Id,
            Referrer = referrer,
            UserAgent = userAgent,
            IpHash = ip
        });
        await _db.SaveChangesAsync();
    }

    public async Task<object?> GetStatsAsync(int id)
    {
        var shortUrl = await _db.ShortUrls
            .Include(s => s.Clicks)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shortUrl == null) return null;

        return new
        {
            shortUrl.Id,
            shortUrl.Code,
            shortUrl.OriginalUrl,
            ClickCount = shortUrl.Clicks.Count,
            RecentClicks = shortUrl.Clicks
                .OrderByDescending(c => c.Timestamp)
                .Take(10)
                .Select(c => new { c.Timestamp, c.Referrer, c.UserAgent })
        };
    }
}
