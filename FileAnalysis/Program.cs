using Microsoft.EntityFrameworkCore;
using FileAnalysis.Data;
using FileAnalysis.Core;

// запуск микросервиса анализа файлов на плагиат
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AnalyseContext>(options =>
    options.UseInMemoryDatabase("AnalysisDb"));

builder.Services.AddScoped<TextExtractor>();
builder.Services.AddScoped<PlagDetector>();
var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileStorage API V1");
    });
}

app.MapControllers();
app.Run();