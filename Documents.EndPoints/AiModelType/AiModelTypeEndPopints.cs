using Documents.Contract.DocumentPrediction;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Documents.EndPoints.AiModelType;

internal static class AiModelTypeEndPopints
{
    public static IEndpointRouteBuilder AddAiModelTypeEndPopints(this IEndpointRouteBuilder apiEndPoint)
    {
        var aiModelTypeEndPoints = apiEndPoint
            .MapGroup($"/aiModelTypes");

        aiModelTypeEndPoints.MapGet("", () =>
        {
            var aiModelTypes = Enum.GetValues<Contract.Model.Enums.AiModel.AiModelType>();

            return Results.Ok(aiModelTypes);
        });

        aiModelTypeEndPoints.MapGet("/test-accuracy", async (
            [FromServices] IDocumentPredictionQueryService documentPredictionQueryService
            ) =>
        {
            var aiModelPredictions = await documentPredictionQueryService.GetAiModelTestAccuracy();

            return Results.Ok(aiModelPredictions.ToArray());
        });

        return aiModelTypeEndPoints;
    }
}
