using System.Text;

namespace FileAnalysis.Core;

public class TextExtractor
{
    // переводим текст в UTF8
    public string ExtractText(byte[] content, string fileName)
    {
        if (content.Length == 0)
        {
            return string.Empty;
        }
        try
        {
            return Encoding.UTF8.GetString(content);
        }
        catch
        {
            return string.Empty;
        }
    }
}