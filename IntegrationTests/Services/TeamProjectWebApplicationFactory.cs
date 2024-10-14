using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace IntegrationTests.Services;

public class TeamProjectWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("YourDatabaseName");
            });

            var context = CreateDbContext(services);
            context.Database.EnsureCreated();
        });
    }

    private static AppDbContext CreateDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return context;
    }
}