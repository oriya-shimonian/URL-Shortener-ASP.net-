using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Domain;

namespace UrlShortener.Api.Data;

// DbContext = "השער" ל-DB. EF Core משתמש בו כדי לדעת אילו טבלאות יש ואיך לקנפג אותן.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // DbSet<T> = "טבלה" לוגית (EF יוצר טבלאות בהתאם למודלים)
    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();
    public DbSet<Click> Clicks => Set<Click>();

    // קונפיגורציה של מסד הנתונים (אינדקסים, קשרים, מחיקות Cascade וכו')
    protected override void OnModelCreating(ModelBuilder b)
    {
        // אינדקס ייחודי על Code כדי שלא יהיו שני קישורים עם אותו קוד
        b.Entity<ShortUrl>()
            .HasIndex(x => x.Code)
            .IsUnique();

        // קשר 1-ל-רבים: ShortUrl אחד -> הרבה Clicks
        b.Entity<Click>()
            .HasOne(c => c.ShortUrl)
            .WithMany(s => s.Clicks)
            .HasForeignKey(c => c.ShortUrlId)
            .OnDelete(DeleteBehavior.Cascade); // אם מוחקים ShortUrl - כל ה-Clicks שלו יימחקו
    }
}
