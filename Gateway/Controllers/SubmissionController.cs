using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared;

namespace Gateway.Controllers;

// контроллер для приема файлов от клиентов, их загрузки в хранилище и запроса анализа

[ApiController]
[Route("api/[controller]")]
public class SubmissionController : ControllerBase
{
    private readonly HttpStorageClient _storageClient;
    private readonly HttpAnalysisClient _analysisClient;
    public SubmissionController(HttpStorageClient storageClient, HttpAnalysisClient analysisClient)
    {
        _storageClient = storageClient;
        _analysisClient = analysisClient;
    }
   
    [HttpPost]
    public async Task<IActionResult> SubmitWork([FromBody] FileData filedata)
    {
        if (string.IsNullOrEmpty(filedata.FileName) || filedata.Content.Length == 0)
        {
            return BadRequest("Некорректные данные файла.");
        }
        try
        {
            Guid storedFileId = await _storageClient.UploadFileAsync(filedata);
            filedata.Id = storedFileId;

            var analysisResult = await _analysisClient.ScanFileAsync(filedata);

            return Ok(analysisResult);
        }
        catch (Exception ex) when (
            ex is HttpRequestException ||
            ex is TaskCanceledException ||
            ex is TimeoutException)
        {
            return StatusCode(503, new { error = "Сервис недоступен. Повторите попытку позже." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }
}