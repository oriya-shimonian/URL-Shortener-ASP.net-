namespace UrlShortener.Api.Domain;

public class Click
{
    public int Id { get; set; }               // מפתח ראשי
    public int ShortUrlId { get; set; }       // FK ל-ShortUrl
    public ShortUrl ShortUrl { get; set; } = default!; // ניווט חזרה ל-ShortUrl

    public DateTime Timestamp { get; set; } = DateTime.UtcNow; // מתי לחצו
    public string? Referrer { get; set; }    // מאיזה עמוד הגיעו (אם יש)
    public string? UserAgent { get; set; }   // דפדפן/קליינט
    public string? IpHash { get; set; }      // HASH של ה-IP (פרטיות)
}
