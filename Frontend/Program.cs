using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend;

// запуск фронтенда

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var gatewayUrl = builder.Configuration["GatewayUrl"] ?? "http://localhost:5000";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(gatewayUrl) });

await builder.Build().RunAsync();