using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Documents.Database.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<DocumentContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DocumentContext>>();

        await using var context = await contextFactory.CreateDbContextAsync();

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var pendingMigrationsList = pendingMigrations.ToArray();

        if (pendingMigrationsList.Length == 0)
        {
            return;
        }
        
        await context.Database.MigrateAsync();
    }
}
