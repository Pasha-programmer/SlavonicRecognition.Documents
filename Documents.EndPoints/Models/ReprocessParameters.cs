using Documents.Contract.Model;

namespace Documents.EndPoints.Models;

internal record ReprocessParameters
{
    public long[] DocumentIds { get; set; }

    public AiModelType ModelType { get; set; }
}
