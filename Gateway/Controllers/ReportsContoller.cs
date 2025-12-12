using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared;

namespace Gateway.Controllers;

// контроллер для получения отчетов анализа файлов

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly HttpAnalysisClient _analysisClient;

    public ReportsController(HttpAnalysisClient analysisClient)
    {
        _analysisClient = analysisClient;
    }
    
    // GET: api/reports
    [HttpGet]
    public async Task<ActionResult<List<AnalysisResult>>> GetAll()
    {
        try
        {
            var reports = await _analysisClient.GetAllReportsAsync();
            return Ok(reports);
        }
        catch (Exception ex) when (
            ex is HttpRequestException ||
            ex is TaskCanceledException ||
            ex is TimeoutException)
        {
            return StatusCode(503, "Сервис анализа недоступен (Timeout/Network Error).");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Gateway Error: {ex.Message}");
        }
    }

    [HttpGet("works/{workId}/reports")]
    public async Task<ActionResult<List<AnalysisResult>>> GetReportsByWork(string workId)
    {
        try
        {
            var reports = await _analysisClient.GetReportsByWorkAsync(workId);
            return Ok(reports);
        }
        catch (Exception ex) when (
            ex is HttpRequestException ||
            ex is TaskCanceledException ||
            ex is TimeoutException)
        {
            return StatusCode(503, "Сервис анализа недоступен (Timeout/Network Error).");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Gateway Error: {ex.Message}");
        }
    }
}