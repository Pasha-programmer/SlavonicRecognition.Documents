using Documents.Contract.Model.AiModelTuning;
using Documents.Contract.TuneAiModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Documents.EndPoints.AiModelTuning;

internal static class AiModelTuningEndPopints
{
    public static IEndpointRouteBuilder AddAiModelTuningEndPopints(this IEndpointRouteBuilder apiEndPoint)
    {
        var aiModelTypeEndPoints = apiEndPoint
            .MapGroup($"/aiModelTuning");

        aiModelTypeEndPoints.MapPost("", async (
            [FromServices] IAiModelTuningService aiModelTuningService,
            [FromBody] AiModelToTuningDto aiModelToTuningDto,
            CancellationToken cancellationToken
            ) =>
        {
            await aiModelTuningService.StartTuneAiModel(aiModelToTuningDto, cancellationToken);

            return Results.Ok(true);
        });

        return aiModelTypeEndPoints;
    }
}
