using Documents.Contract.Model.Document;

namespace Documents.Contract.Document;

public interface IDocumentCommandService
{
    Task<long> AddDocument(DocumentToCreateDto model, CancellationToken cancellationToken = default);

    Task<bool> DeleteDocuments(IReadOnlyCollection<long> documentIds, CancellationToken cancellationToken = default);
}
