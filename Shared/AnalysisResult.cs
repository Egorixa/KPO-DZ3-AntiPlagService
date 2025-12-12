namespace Shared;

// результат анализа файла на плагиат

public class AnalysisResult
{
    public Guid FileId { get; set; }
    public int PlagPercent { get; set; }
    public bool IsPlag { get; set; }
    public List<string> TopWords { get; set; } = new List<string>();
    public Guid? SourceOfPlagId { get; set; }
    public DateTime CheckedAt { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AssignmentName { get; set; } = string.Empty;
}