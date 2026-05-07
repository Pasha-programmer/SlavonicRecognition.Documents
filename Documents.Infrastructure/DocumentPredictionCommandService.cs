using Documents.Contract;
using Documents.Contract.Model;
using Documents.Database;
using Documents.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure;

internal class DocumentPredictionCommandService(
    IDbContextFactory<DocumentContext> contextFactory) 
    : IDocumentPredictionCommandService
{
    /// <inheritdoc/>
    public async Task<bool> AddPredication(IReadOnlyCollection<RecognitionResult> recognitionResult, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        context.AttachRange(recognitionResult.Select(rr => new DocumentPrediction
        {
            DocumentId = rr.DocumentId,
            Value = rr.Label,
            Prob = rr.Probability,
        }));

        return (await context.SaveChangesAsync(cancellationToken)) > 0;
    }
}
