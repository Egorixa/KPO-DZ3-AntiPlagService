using System.ComponentModel.DataAnnotations;
namespace FileStorage.Entities;

// сущность для хранения загруженного файла
public class StoredFile
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string FileName { get; set; } = string.Empty;
    [Required]
    public string AuthorName { get; set; } = string.Empty;
    [Required]
    public string AssignmentName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>(); 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}