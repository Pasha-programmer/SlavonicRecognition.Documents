using Documents.Contract.Model.Enums.AiModel;

namespace Documents.Contract.Model.Document;

public record DocumentToCreateDto
{
    public required string FileName { get; set; }

    public required byte[] FileBlob { get; set; }

    public required AiModelType SelectedModelType { get; set; }
}
