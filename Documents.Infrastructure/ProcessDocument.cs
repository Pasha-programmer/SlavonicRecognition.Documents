using Documents.Contract;

namespace Documents.Infrastructure;

internal class ProcessDocument : IProcessDocument
{
    private readonly IRabbitMqService _rabbitMqService;

    private const string QUEUE_NAME = "Document.Queue";

    public ProcessDocument(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    /// <inheritdoc/>
    public async Task StartProcessDocument(long documentId, Memory<byte> blob, CancellationToken cancellationToken = default)
    {
        await _rabbitMqService.SendMessage(QUEUE_NAME, new
        {
            DocumentId = documentId,
            Blob = blob
        }, cancellationToken);
    }
}
