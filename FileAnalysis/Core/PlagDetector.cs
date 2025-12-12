using System.Text.RegularExpressions;

namespace FileAnalysis.Core;

public class PlagDetector
{
    // делим пересечение на объединение множеств слов
    public (double maxScore, Guid? similarDocId) Compare(string newText, Dictionary<Guid, string> previousDocuments)
    {
        var newWords = Tokenize(newText);
        if (newWords.Count == 0)
        {
            return (0, null);
        }
        double maxScore = 0.0;
        Guid? bestMatchId = null;
        foreach (var doc in previousDocuments)
        {
            var oldWords = Tokenize(doc.Value);
            var intersection = newWords.Intersect(oldWords).Count();
            var union = newWords.Union(oldWords).Count();
            if (union == 0)
            {
                continue;
            }
            double score = (double)intersection / union;
            if (score > maxScore)
            {
                maxScore = score;
                bestMatchId = doc.Key;
            }
        }
        return (maxScore, bestMatchId);
    }
    
    private HashSet<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) {
            return new HashSet<string>();
        }
        return Regex.Matches(text.ToLowerInvariant(), @"\w+")
            .Select(m => m.Value)
            .ToHashSet();
    }
}