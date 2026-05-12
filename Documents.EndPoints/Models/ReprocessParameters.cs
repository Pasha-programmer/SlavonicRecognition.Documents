using Documents.Contract.Model;

namespace Documents.EndPoints.Models;

internal record ReprocessParameters
{
    public long DocumentId { get; set; }

    public AiModelType ModelType { get; set; }
}
