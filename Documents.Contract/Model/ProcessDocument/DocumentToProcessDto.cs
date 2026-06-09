using Documents.Contract.Model.Enums.AiModel;

namespace Documents.Contract.Model.ProcessDocument;

public record DocumentToProcessDto
{
    public required long DocumentId { get; set; }

    public required Memory<byte> Blob { get; set; }

    public required AiModelType AiModelType { get; set; }

    public required bool UseTunedModels { get; set; }
}
