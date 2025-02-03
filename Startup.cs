using Microsoft.Extensions.DependencyInjection;
using Domain.Abstractions;
using Infrastructure.Persistence;

namespace WebApi;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ... existing code ...
    }
} 