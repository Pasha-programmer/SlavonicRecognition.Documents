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
    public async Task StartProcessDocument(long documentId, Memory<byte> blob, AiModelType aiModelType, CancellationToken cancellationToken = default)
    {
        await _rabbitMqService.SendMessage(RECOGNITION_REQUEST_QUEUE_NAME, new
        {
            DocumentId = documentId,
            Blob = blob,
            Model = Enum.GetName(aiModelType),
        }, cancellationToken);
    }
}
