using Documents.Contract.Model.Document;

namespace Documents.Contract.Document;

public interface IDocumentQueryService
{
    Task<IReadOnlyCollection<DocumentDto?>> GetDocuments(IReadOnlyCollection<long>? documentIds, CancellationToken cancellationToken = default);
}
