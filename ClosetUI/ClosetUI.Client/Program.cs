using ClosetUI.Models.Services;
using ClosetUI.Models.Services.Interfaces;
using ClosetUI.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<IPartCalculationService, ParamsClientService>();
builder.Services.AddScoped<IBoardDrawingService, BoardDrawingService>();
builder.Services.AddScoped(http => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

await builder.Build().RunAsync();
