using Microsoft.EntityFrameworkCore;
using FileStorage.Data;

// запуск микросервиса хранения файлов

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<StorageDbContext>(options =>
    options.UseInMemoryDatabase("FileStorageDb"));

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileStorage API V1");
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();