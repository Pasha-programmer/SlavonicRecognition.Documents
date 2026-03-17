using Documents.Infrastructure.DependencyInjection;

namespace Documents.WebApi.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);

        services.AddHealthChecks();

        return services;
    }
}
