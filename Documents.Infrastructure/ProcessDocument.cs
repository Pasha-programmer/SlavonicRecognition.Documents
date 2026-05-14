using Documents.Contract;
using Documents.Contract.Model;

namespace Documents.Infrastructure;

internal class ProcessDocument : IProcessDocument
{
    private readonly IRabbitMqService _rabbitMqService;

    private const string RECOGNITION_REQUEST_QUEUE_NAME = "RecognitionRequest.Queue";

    public ProcessDocument(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    /// <inheritdoc/>
    public async Task StartProcessDocument(ProcessingDocument processingDocument, CancellationToken cancellationToken = default)
    {
        await _rabbitMqService.SendMessage(RECOGNITION_REQUEST_QUEUE_NAME, processingDocument, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task StartProcessDocuments(IReadOnlyCollection<ProcessingDocument> processingDocuments, CancellationToken cancellationToken = default)
    {
        foreach (var processingDocument in processingDocuments)
        {
            await _rabbitMqService.SendMessage(RECOGNITION_REQUEST_QUEUE_NAME, processingDocument, cancellationToken);
        }
    }
}
