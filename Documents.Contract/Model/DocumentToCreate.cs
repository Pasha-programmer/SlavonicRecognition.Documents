namespace Documents.Contract.Model;

public record DocumentToCreate
{
    public required string FileName { get; set; }

    public required byte[] FileBlob { get; set; }

    public required AiModelType SelectedModelType { get; set; }
}
