using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

/// <summary>
/// Extension methods for setting up the application's infrastructure services.
/// This includes configuring the database context, Identity services, and the DbInitializer.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Adds infrastructure services, including database context and Identity configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration containing settings like connection strings.</param>
    /// <param name="environment">The current hosting environment (for environment-based configuration).</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for dependency injection.</returns>
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