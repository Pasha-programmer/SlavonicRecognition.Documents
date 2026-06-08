using Documents.Contract.Model.TunedDocumentPrediction;
using Documents.Contract.TunedDocumentPrediction;
using Documents.Database;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.TunedDocumentPrediction;

public class TunedDocumentPredictionCommandService(
    IDbContextFactory<DocumentContext> contextFactory
    ) : ITunedDocumentPredictionCommandService
{
    private readonly IDbContextFactory<DocumentContext> _contextFactory = contextFactory;

    /// <inheritdoc/>
    public async Task<bool> AddTunedPredication(IReadOnlyCollection<TunedPrediction> tunedPrediction, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        context.AttachRange(tunedPrediction.Select(rr => new Database.Entity.TunedDocumentPrediction
        {
            DocumentPredictionId = rr.DocumentPredictionId,
            ModelType = (int)rr.ModelType,
        }));

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
