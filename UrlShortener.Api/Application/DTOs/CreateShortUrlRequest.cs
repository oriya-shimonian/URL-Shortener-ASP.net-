namespace UrlShortener.Api.Application.DTOs;

public class CreateShortUrlRequest
{
    public string OriginalUrl { get; set; } = string.Empty;
}
