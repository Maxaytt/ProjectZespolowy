using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

/// <summary>
/// Initializes the database by applying pending migrations on application startup.
/// </summary>
public class DbInitializer(IServiceScopeFactory scopeFactory) : IHostedService
{
    /// <summary>
    /// Applies pending migrations to the database on application startup.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // Применение миграций
        await context.Database.MigrateAsync(cancellationToken);

        // Создание ролей
        var roleNames = new[] { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }

    /// <summary>
    /// Task to stop the hosted service.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}