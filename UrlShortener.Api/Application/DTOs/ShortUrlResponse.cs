namespace UrlShortener.Api.Application.DTOs;

public class ShortUrlResponse
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string OriginalUrl { get; init; } = string.Empty;
    public string ShortUrl { get; init; } = string.Empty;
    public string OpenUrl { get; init; } = string.Empty;
    public string Preview { get; init; } = string.Empty;
}
