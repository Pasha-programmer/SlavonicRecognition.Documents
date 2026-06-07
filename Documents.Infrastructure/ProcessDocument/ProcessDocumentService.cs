using Documents.Contract.Model.ProcessDocument;
using Documents.Contract.ProcessDocument;
using Documents.Infrastructure.Contract;

namespace Documents.Infrastructure.ProcessDocument;

internal class ProcessDocumentService(IRabbitMqService rabbitMqService) : IProcessDocumentService
{
    private readonly IRabbitMqService _rabbitMqService = rabbitMqService;

    private const string RECOGNITION_REQUEST_QUEUE_NAME = "RecognitionRequest.Queue";

    /// <inheritdoc/>
    public async Task StartProcessDocument(DocumentToProcessDto processingDocument, CancellationToken cancellationToken = default)
    {
        await StartProcessDocuments([processingDocument], cancellationToken);
    }

    /// <inheritdoc/>
    public async Task StartProcessDocuments(IReadOnlyCollection<DocumentToProcessDto> processingDocuments, CancellationToken cancellationToken = default)
    {
        foreach (var processingDocument in processingDocuments)
        {
            await _rabbitMqService.SendMessage(RECOGNITION_REQUEST_QUEUE_NAME, processingDocument, cancellationToken);
        }
    }
}
