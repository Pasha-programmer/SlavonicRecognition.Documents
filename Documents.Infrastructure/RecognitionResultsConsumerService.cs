using Documents.Contract;
using Documents.Contract.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Documents.Infrastructure;

internal class RecognitionResultsConsumerService : IHostedService
{
    private readonly ILogger<RecognitionResultsConsumerService> _logger;
    private readonly IDocumentPredictionQueryService _documentPredictionQueryService;
    private IConnection _connection;
    private IChannel _channel;

    private const string RECOGNITION_RESULTS_QUEUE_NAME = "RecognitionResults.Queue";

    public RecognitionResultsConsumerService(
        ILogger<RecognitionResultsConsumerService> logger,
        IDocumentPredictionQueryService documentPredictionQueryService)
    {
        _logger = logger;
        _documentPredictionQueryService = documentPredictionQueryService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запуск сервиса потребителя результатов распознавания");

        await InitializeRabbitMqAsync(cancellationToken);
        await StartConsumingAsync(cancellationToken);
    }

    private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest",
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: RECOGNITION_RESULTS_QUEUE_NAME,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(0, 1, false, cancellationToken);
    }

    private async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());
                var result = JsonSerializer.Deserialize<RecognitionResult>(message);

                if (result != null)
                {
                    await _documentPredictionQueryService.AddPredication(result, cancellationToken);
                }

                await _channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки сообщения");
                await _channel.BasicNackAsync(args.DeliveryTag, false, true, cancellationToken);
            }
        };

        await _channel.BasicConsumeAsync(RECOGNITION_RESULTS_QUEUE_NAME, false, consumer, cancellationToken);
        _logger.LogInformation("Consumer начал прослушивание очереди {QueueName}", RECOGNITION_RESULTS_QUEUE_NAME);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Остановка сервиса потребителя результатов распознавания");

        if (_channel != null && _channel.IsOpen)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        if (_connection != null && _connection.IsOpen)
        {
            await _connection.CloseAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
