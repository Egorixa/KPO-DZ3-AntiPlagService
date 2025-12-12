using System.ComponentModel.DataAnnotations;

namespace FileAnalysis.Entities;

// сущность для хранения проанализированного документа
public class AnalyzedDocument
{
    [Key]
    public Guid FileId { get; set; }
    public string TextContent { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}