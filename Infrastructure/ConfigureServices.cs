using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionStringKey = environment.IsEnvironment("Test") 
            ? "TestConnection" 
            : "DefaultConnection";

        var connectionString = configuration.GetConnectionString(connectionStringKey)
               ?? throw new InvalidOperationException($"{connectionStringKey} does not exist");
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("Infrastructure")));

        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        return services.AddHostedService<DbInitializer>();
    }
}