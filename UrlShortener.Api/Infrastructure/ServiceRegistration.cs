using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(config.GetConnectionString("Default")));

        return services;
    }
}
