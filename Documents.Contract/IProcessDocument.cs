namespace Documents.Contract;

public interface IProcessDocument
{
    public Task StartProcessDocument(long documentId, CancellationToken cancellationToken = default);
}
