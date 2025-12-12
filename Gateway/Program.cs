using Polly;
using Polly.Extensions.Http;

// запуск сервиса-посредника (API Gateway) для взаимодействия с микросервисами хранилища и анализа файлов

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var fastRetryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<TimeoutException>()
    .WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(500));

builder.Services.AddHttpClient<Gateway.Services.HttpStorageClient>(client =>
{
    var url = builder.Configuration["Microservices:StorageUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(url);
    client.Timeout = TimeSpan.FromSeconds(3);
})
.AddPolicyHandler(fastRetryPolicy);

builder.Services.AddHttpClient<Gateway.Services.HttpAnalysisClient>(client =>
{
    var url = builder.Configuration["Microservices:AnalysisUrl"] ?? "http://localhost:5002";
    client.BaseAddress = new Uri(url);
    client.Timeout = TimeSpan.FromSeconds(3);
})
.AddPolicyHandler(fastRetryPolicy);
var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();