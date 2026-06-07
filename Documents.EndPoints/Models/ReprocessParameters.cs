namespace Documents.EndPoints.Models;

internal record ReprocessParameters
{
    public long[] DocumentIds { get; set; }

    public Contract.Model.Enums.AiModel.AiModelType ModelType { get; set; }
}
