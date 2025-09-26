namespace UrlShortener.Api.Domain;

public class ShortUrl
{
    public int Id { get; set; }                       // מפתח ראשי (Identity)
    public string Code { get; set; } = default!;      // הקוד המקוצר (למשל: "abc123")
    public string OriginalUrl { get; set; } = default!; // ה-URL המלא להפניה
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ניווט: כל ShortUrl יכול להכיל הרבה קליקים
    public ICollection<Click> Clicks { get; set; } = new List<Click>();
}
