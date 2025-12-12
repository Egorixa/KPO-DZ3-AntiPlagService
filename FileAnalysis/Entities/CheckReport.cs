using System.ComponentModel.DataAnnotations;

namespace FileAnalysis.Entities;

// сущность для хранения отчета о проверке на плагиат
public class CheckReport
{
    [Key]
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public int SimilarityPercent { get; set; }
    public bool IsPlagiarism { get; set; }
    public Guid? MostSimilarFileId { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public string AuthorName { get; set; } = string.Empty;
    public string AssignmentName { get; set; } = string.Empty;
}