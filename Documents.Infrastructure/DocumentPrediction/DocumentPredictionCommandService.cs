using Documents.Contract.DocumentPrediction;
using Documents.Contract.Model.DocumentPrediction;
using Documents.Database;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.DocumentPrediction;

internal class DocumentPredictionCommandService(
    IDbContextFactory<DocumentContext> contextFactory)
    : IDocumentPredictionCommandService
{
    /// <inheritdoc/>
    public async Task<bool> AddPredication(IReadOnlyCollection<RecognitionResultDto> recognitionResult, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        context.AttachRange(recognitionResult.Select(rr => new Database.Entity.DocumentPrediction
        {
            DocumentId = rr.DocumentId,
            ModelType = (int?)rr.ModelType,
            RecognitionType = (int)rr.RecognitionType,
            Value = rr.Label,
            Prob = rr.Probability,
        }));

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
