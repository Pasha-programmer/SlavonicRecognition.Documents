using Documents.Contract.Model;

namespace Documents.Contract;

public interface IDocumentCommandService
{
    Task<long> AddDocument(DocumentToCreate model, CancellationToken cancellationToken = default);
}
