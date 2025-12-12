using Shared;

namespace Gateway.Services;

// клиент для взаимодействия с сервисом анализа файлов по HTTP

public class HttpAnalysisClient
{
    private readonly HttpClient _httpClient;

    public HttpAnalysisClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<AnalysisResult?> ScanFileAsync(FileData filedata)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/scan", filedata);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AnalysisResult>();
    }
    
    public async Task<AnalysisResult?> GetReportAsync(Guid fileId)
    {
        var response = await _httpClient.GetAsync($"/api/scan/reports/{fileId}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<AnalysisResult>();
    }
    
    public async Task<List<AnalysisResult>> GetAllReportsAsync()
    {
        var response = await _httpClient.GetAsync("/api/scan/reports");
    
        if (!response.IsSuccessStatusCode)
        {
            return new List<AnalysisResult>();
        }

        return await response.Content.ReadFromJsonAsync<List<AnalysisResult>>() 
               ?? new List<AnalysisResult>();
    }

    public async Task<List<AnalysisResult>> GetReportsByWorkAsync(string workId)
    {
        var encodedId = System.Net.WebUtility.UrlEncode(workId);
        var response = await _httpClient.GetAsync($"/api/scan/works/{encodedId}/reports");

        if (!response.IsSuccessStatusCode)
        {
            return new List<AnalysisResult>();
        }

        return await response.Content.ReadFromJsonAsync<List<AnalysisResult>>()
               ?? new List<AnalysisResult>();
    }
}