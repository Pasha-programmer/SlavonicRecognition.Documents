using Documents.Contract.Model.ProcessDocument;

namespace Documents.Contract.ProcessDocument;

public interface IProcessDocumentService
{
    public Task StartProcessDocument(DocumentToProcessDto processingDocument, CancellationToken cancellationToken = default);

    public Task StartProcessDocuments(IReadOnlyCollection<DocumentToProcessDto> processingDocuments, CancellationToken cancellationToken = default);
}
