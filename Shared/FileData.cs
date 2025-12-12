namespace Shared;

// модель данных файла, передаваемая между сервисами
public class FileData
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AssignmentName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
}