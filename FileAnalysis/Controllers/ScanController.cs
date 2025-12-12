using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using FileAnalysis.Core;
using FileAnalysis.Data;
using FileAnalysis.Entities;
using System.Text.RegularExpressions;

namespace FileAnalysis.Controllers;

// контроллер для анализа файлов на плагиат
[ApiController]
[Route("api/[controller]")]
public class ScanController : ControllerBase
{
    private readonly AnalyseContext _context;
    private readonly TextExtractor _extractor;
    private readonly PlagDetector _detector;

    public ScanController(AnalyseContext context, TextExtractor extractor, PlagDetector detector)
    {
        _context = context;
        _extractor = extractor;
        _detector = detector;
    }

    private List<string> GetTopWords(string text, int count = 50)
    {
        if (string.IsNullOrWhiteSpace(text)) {
            return new List<string>();
        }

        var allWords = Regex.Matches(text.ToLowerInvariant(), @"\w+")
            .Select(m => m.Value)
            .ToList();

        if (allWords.Count == 0) {
            return new List<string>();
        }

        var filteredWords = allWords.Where(w => w.Length > 3).ToList();

        if (filteredWords.Count < 5) {
            filteredWords = allWords.Where(w => w.Length >= 2).ToList();
        }

        if (filteredWords.Count == 0) {
            filteredWords = allWords;
        }

        var wordStats = filteredWords
            .GroupBy(w => w)
            .Select(g => new { Word = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(count)
            .ToList();

        if (wordStats.Count == 0)
        {
            return new List<string>();
        }

        int maxCount = wordStats.First().Count;
        var result = new List<string>();

        foreach (var item in wordStats)
        {
            int scaleFactor = allWords.Count < 20 ? 5 : 20;
            int repeatCount = (int)Math.Ceiling((double)item.Count / maxCount * scaleFactor);
            result.AddRange(Enumerable.Repeat(item.Word, repeatCount));
        }
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<AnalysisResult>> Scan([FromBody] FileData filedata)
    {
        string currentText = _extractor.ExtractText(filedata.Content, filedata.FileName);

        var sameAssignmentFileIds = await _context.Reports
            .Where(r => r.AssignmentName == filedata.AssignmentName)
            .Select(r => r.FileId)
            .ToListAsync();

        var previousDocs = await _context.Documents
            .Where(d => sameAssignmentFileIds.Contains(d.FileId))
            .ToDictionaryAsync(d => d.FileId, d => d.TextContent);

        var (score, bestMatchId) = _detector.Compare(currentText, previousDocs);
        int percent = (int)(score * 100);
        bool isPlag = percent > 60;
        var report = new CheckReport
        {
            Id = Guid.NewGuid(),
            FileId = filedata.Id,
            SimilarityPercent = percent,
            IsPlagiarism = isPlag,
            MostSimilarFileId = bestMatchId,
            AuthorName = filedata.AuthorName,
            AssignmentName = filedata.AssignmentName
        };
        _context.Reports.Add(report);

        if (!_context.Documents.Any(d => d.FileId == filedata.Id))
        {
            _context.Documents.Add(new AnalyzedDocument { FileId = filedata.Id, TextContent = currentText });
        }
        await _context.SaveChangesAsync();

        var topWords = GetTopWords(currentText);
        return Ok(new AnalysisResult
        {
            FileId = filedata.Id,
            PlagPercent = percent,
            IsPlag = isPlag,
            SourceOfPlagId = bestMatchId,
            TopWords = topWords
        });
    }

    [HttpGet("reports")]
    public async Task<ActionResult<List<AnalysisResult>>> GetAllReports()
    {
        var reports = await _context.Reports.OrderByDescending(r => r.CheckedAt).ToListAsync();

        var fileIds = reports.Select(r => r.FileId).ToList();
        var docs = await _context.Documents
            .Where(d => fileIds.Contains(d.FileId))
            .ToDictionaryAsync(d => d.FileId, d => d.TextContent);

        var dtos = reports.Select(r => new AnalysisResult
        {
            FileId = r.FileId,
            PlagPercent = r.SimilarityPercent,
            IsPlag = r.IsPlagiarism,
            SourceOfPlagId = r.MostSimilarFileId,
            CheckedAt = r.CheckedAt,
            AuthorName = r.AuthorName,
            AssignmentName = r.AssignmentName,
            TopWords = docs.ContainsKey(r.FileId) ? GetTopWords(docs[r.FileId]) : new List<string>()
        }).ToList();

        return Ok(dtos);
    }

    [HttpGet("works/{workId}/reports")]
    public async Task<ActionResult<List<AnalysisResult>>> GetReportsByAssignment(string workId)
    {
        var decodedName = System.Net.WebUtility.UrlDecode(workId);

        var reports = await _context.Reports
            .Where(r => r.AssignmentName == decodedName)
            .OrderByDescending(r => r.CheckedAt)
            .ToListAsync();

        var fileIds = reports.Select(r => r.FileId).ToList();
        var docs = await _context.Documents
            .Where(d => fileIds.Contains(d.FileId))
            .ToDictionaryAsync(d => d.FileId, d => d.TextContent);

        var dtos = reports.Select(r => new AnalysisResult
        {
            FileId = r.FileId,
            PlagPercent = r.SimilarityPercent,
            IsPlag = r.IsPlagiarism,
            SourceOfPlagId = r.MostSimilarFileId,
            CheckedAt = r.CheckedAt,
            AuthorName = r.AuthorName,
            AssignmentName = r.AssignmentName,
            TopWords = docs.ContainsKey(r.FileId) ? GetTopWords(docs[r.FileId]) : new List<string>()
        }).ToList();

        return Ok(dtos);
    }
}