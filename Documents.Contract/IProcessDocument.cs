namespace Documents.Contract;

public interface IProcessDocument
{
    public Task StartProcessDocument(long documentId, Memory<byte> blob, CancellationToken cancellationToken = default);
}
