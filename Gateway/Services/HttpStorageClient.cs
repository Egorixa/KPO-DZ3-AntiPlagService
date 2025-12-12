using Shared;

namespace Gateway.Services;

// клиент для взаимодействия с сервисом хранилища файлов по HTTP
public class HttpStorageClient
{
    private readonly HttpClient _httpClient;

    public HttpStorageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Guid> UploadFileAsync(FileData filedata)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/files/upload", filedata);
        response.EnsureSuccessStatusCode();
        var fileId = await response.Content.ReadFromJsonAsync<Guid>();
        return fileId;
    }
}