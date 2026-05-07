using Documents.Contract.Model;

namespace Documents.Contract;

public interface IDocumentQueryService
{
    Task<DocumentDto?> GetDocument(long documentId, CancellationToken cancellationToken = default);
}
