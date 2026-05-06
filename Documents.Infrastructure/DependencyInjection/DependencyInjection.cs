using Documents.Contract;
using Documents.Database.DependencyInjection;
using Documents.Infrastructure.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Documents.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseServices(configuration);

        services.AddOptions<RabbitMqConfigurationOption>()
            .Bind(configuration.GetSection("RabbitMq"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RabbitMqConfigurationOption>>().Value;
            return new ConnectionFactory()
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                //RequestedHeartbeat = TimeSpan.FromSeconds(60),
                //AutomaticRecoveryEnabled = true,
                //NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                //TopologyRecoveryEnabled = true,
                //ContinuationTimeout = TimeSpan.FromSeconds(20),
            }; 
        });

        services.AddScoped<IRabbitMqService, RabbitMqService>();
        services.AddScoped<IProcessDocument, ProcessDocument>();
        services.AddScoped<IDocumentCommandService, DocumentCommandService>();
        services.AddScoped<IDocumentPredictionQueryService, DocumentPredictionQueryService>();

        services.AddHostedService<RecognitionResultsConsumerService>();

        return services;
    }
}
