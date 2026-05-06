using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Documents.Infrastructure;

public class RabbitMqService : IRabbitMqService, IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IChannel _channel;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public RabbitMqService(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private async Task EnsureConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
            return;

        if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
            return;

        _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
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
        await EnsureConnectionAsync(cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(message);
        var props = new BasicProperties();

        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.DisposeAsync();

        if (_connection != null)
            await _connection.DisposeAsync();
    }
}
