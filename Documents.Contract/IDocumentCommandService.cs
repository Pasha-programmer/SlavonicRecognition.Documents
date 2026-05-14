using Documents.Contract.Model;

namespace Documents.Contract;

public interface IDocumentCommandService
{
    Task<long> AddDocument(DocumentToCreate model, CancellationToken cancellationToken = default);

    Task<bool> DeleteDocuments(IReadOnlyCollection<long> documentIds, CancellationToken cancellationToken = default);
}
