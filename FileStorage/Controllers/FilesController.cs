using Microsoft.AspNetCore.Mvc;
using Shared;
using FileStorage.Data;
using FileStorage.Entities;

namespace FileStorage.Controllers;

// контроллер для загрузки и получения файлов из хранилища

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly StorageDbContext _context;

    public FilesController(StorageDbContext context)
    {
        _context = context;
    }

    // POST: api/files/upload
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] FileData filedata)
    {
        if (filedata.Content.Length == 0) {
            return BadRequest("File content is empty");
        }

        if (string.IsNullOrWhiteSpace(filedata.FileName)) {
            return BadRequest("File name is required");
        }

        if (string.IsNullOrWhiteSpace(filedata.AuthorName) || string.IsNullOrWhiteSpace(filedata.AssignmentName)) {
            return BadRequest("AuthorName and AssignmentName are required");
        }

        const int maxFileSizeBytes = 5 * 1024 * 1024;
        if (filedata.Content.Length > maxFileSizeBytes) {
            return BadRequest("File is too large");
        }

        var storedFile = new StoredFile
        {
            Id = Guid.NewGuid(),
            FileName = filedata.FileName,
            AuthorName = filedata.AuthorName,
            AssignmentName = filedata.AssignmentName,
            CreatedAt = DateTime.UtcNow,
            Content = filedata.Content 
        };

        try
        {
            _context.Files.Add(storedFile);
            await _context.SaveChangesAsync();
            return Ok(storedFile.Id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to save file: {ex.Message}");
        }
    }

    // GET: api/files/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<FileData>> Get(Guid id)
    {
        try
        {
            var fileRecord = await _context.Files.FindAsync(id);
            
            if (fileRecord == null)
                return NotFound("File not found in database");

            var result = new FileData
            {
                Id = fileRecord.Id,
                FileName = fileRecord.FileName,
                AuthorName = fileRecord.AuthorName,
                AssignmentName = fileRecord.AssignmentName,
                Content = fileRecord.Content 
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to load file: {ex.Message}");
        }
    }
}