namespace Documents.Infrastructure;

internal interface IRabbitMqService
{
    Task SendMessage(string queueName, object obj, CancellationToken cancellationToken = default);

    Task SendMessage(string queueName, string message, CancellationToken cancellationToken = default);
}