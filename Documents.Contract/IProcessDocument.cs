using Documents.Contract.Model;

namespace Documents.Contract;

public interface IProcessDocument
{
    public Task StartProcessDocument(ProcessingDocument processingDocument, CancellationToken cancellationToken = default);

    public Task StartProcessDocuments(IReadOnlyCollection<ProcessingDocument> processingDocuments, CancellationToken cancellationToken = default);
}
