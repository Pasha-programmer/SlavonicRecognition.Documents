using Documents.Contract.Model;

namespace Documents.Contract;

public interface IDocumentQueryService
{
    Task<IReadOnlyCollection<DocumentDto?>> GetDocuments(IReadOnlyCollection<long>? documentIds, CancellationToken cancellationToken = default);
}
