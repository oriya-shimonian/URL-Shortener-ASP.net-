
``
# URL Shortener with Click Analytics (ASP.NET Core 8 + EF Core + SQLite)

A production-grade yet lightweight **URL Shortener API** built with **ASP.NET Core 8** and **Entity Framework Core**.  
The project demonstrates **clean architecture, modular design, and scalability principles** while remaining easy to run locally.

---

## ✨ Features

- Shorten long URLs into unique codes
- 302 Redirects with **click tracking**
- Basic analytics: click count, referrer, user-agent, recent activity
- REST API with **Swagger/OpenAPI** documentation
- Clean layering: **Controllers → Services → Data → Domain**
- Built with **Dependency Injection, DTOs, and Validators**
- Database: **SQLite** (easy to swap for PostgreSQL or SQL Server)

---

## 🛠 Tech Stack

- **Backend:** ASP.NET Core 8 Web API
- **Database:** Entity Framework Core + SQLite
- **Documentation:** Swagger / OpenAPI
- **Validation:** FluentValidation
- **Other:** Dependency Injection, LINQ, Async/Await

---

## 📂 Project Structure

```

UrlShortener.Api/
│  Program.cs
│  appsettings.json
│
├─ Controllers/
│   └─ ShortUrlsController.cs
│
├─ Services/
│   ├─ ShortUrlService.cs
│   └─ CodeGenerator.cs
│
├─ Application/
│   ├─ DTOs/
│   └─ Validators/
│
├─ Domain/
│   ├─ ShortUrl.cs
│   └─ Click.cs
│
├─ Data/
│   └─ AppDbContext.cs
│
├─ Infrastructure/
│   └─ ServiceCollectionExtensions.cs
│
├─ Migrations/         # EF Core migrations
└─ ScreenShots/  

````

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- SQLite (included by EF Core, no manual install needed)

### Run Locally
```bash
# Restore packages
dotnet restore

# Apply migrations
dotnet ef database update

# Run the API
dotnet run --project UrlShortener.Api
````

Navigate to:

* **Swagger UI:** [http://localhost:5041/swagger](http://localhost:5041/swagger)
* **API root health check:** [http://localhost:5041/](http://localhost:5041/)

---

## 📡 API Endpoints

### 1. Create a short URL

`POST /api/ShortUrls`

Request:

```json
{
  "originalUrl": "https://example.com/some/very/long/link"
}
```

Response:

```json
{
  "id": 1,
  "code": "aB12xY",
  "originalUrl": "https://example.com/some/very/long/link",
  "shortUrl": "http://localhost:5041/aB12xY",
  "openUrl": "http://localhost:5041/go/aB12xY",
  "preview": "http://localhost:5041/api/ShortUrls/1/stats"
}
```

---

### 2. Redirect to original URL

`GET /{code}`
Returns **302 Redirect**.
👉 Best tested directly in the browser or Postman (Swagger may show CORS error, which is expected).

---

### 3. Analytics

`GET /api/ShortUrls/{id}/stats`

Response:

```json
{
  "id": 1,
  "code": "aB12xY",
  "originalUrl": "https://example.com/some/very/long/link",
  "clickCount": 4,
  "recentClicks": [
    {
      "timestamp": "2025-09-26T09:40:07Z",
      "referrer": "http://localhost:5041/swagger",
      "userAgent": "Chrome/..."
    }
  ]
}
```

---

## 🖼 Screenshots

``
![Swagger UI](/UrlShortener.Api/ScreenShots/Swagger UI main screen.png)
![Create Response](/UrlShortener.Api/ScreenShots/POST%20-api-ShortUrls%20response.png)
![Redirect Page](/UrlShortener.Api/ScreenShots/-go-{code}%20redirect%20page.png)
![Stats](/UrlShortener.Api/ScreenShots/Analytics%20response.png)
``
---

## 🌍 Deployment

This project can be seen at:

* [Render](https://url-shortener-ae6w.onrender.com/swagger)
---

## 📜 License

MIT License (or choose another).

```
