using MahnaMahna.Client.Services;
using MahnaMahna.Shared.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<ITodoApiService, TodoApiService>();

//SignalR
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddHttpClient("",(HttpClient client) => { client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); });
await builder.Build().RunAsync();
