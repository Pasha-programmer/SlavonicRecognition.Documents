using Documents.Contract.Model;

namespace Documents.Contract;

public interface IProcessDocument
{
    public Task StartProcessDocument(long documentId, Memory<byte> blob, AiModelType aiModelType, CancellationToken cancellationToken = default);
}
