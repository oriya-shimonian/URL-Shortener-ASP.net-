# ---- Runtime base ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# העתק csproj/פתרון כדי לאפשר restore עם cache
COPY ["UrlShortener.Api/UrlShortener.Api.csproj", "UrlShortener.Api/"]
COPY ["UrlShortener.sln", "./"]
RUN dotnet restore "UrlShortener.Api/UrlShortener.Api.csproj"

# העתק שאר המקור ובנה
COPY . .
WORKDIR "/src/UrlShortener.Api"
RUN dotnet publish "UrlShortener.Api.csproj" -c Release -o /app/publish --no-restore

# ---- Final image ----
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UrlShortener.Api.dll"]
