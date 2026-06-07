using Documents.EndPoints.AiModelType;
using Documents.EndPoints.Document;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Documents.EndPoints;

public static class EndPopint
{
    public static IEndpointRouteBuilder AddEndPoints(this IEndpointRouteBuilder endPoints)
    {
        var apiEndPoint = endPoints
            .MapGroup("/api");

        apiEndPoint.MapHealthChecks("/health");

        DocumentEndPopints.AddDocumentEndPopints(apiEndPoint);

        AiModelTypeEndPopints.AddAiModelTypeEndPopints(apiEndPoint);

        return apiEndPoint;
    }
}
