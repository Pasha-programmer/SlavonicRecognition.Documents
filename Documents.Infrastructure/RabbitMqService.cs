using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Documents.Infrastructure;

internal class RabbitMqService : IRabbitMqService
{
    private readonly IConnectionFactory _connectionFactory;

    public RabbitMqService(IConnectionFactory connectionFactory)
    {
        this._connectionFactory = connectionFactory;
    }

    /// <inheritdoc/>
    public async Task SendMessage(string queueName, object obj, CancellationToken cancellationToken = default)
    {
        var message = JsonSerializer.Serialize(obj);
        await SendMessage(queueName, message, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SendMessage(string queueName, string message, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(message);

        var props = new BasicProperties();

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken);
    }

}
