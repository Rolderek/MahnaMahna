namespace MahnaMahna.Shared.Services;
using MahnaMahna.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

public interface INotificationService
{
    event Action<TodoItem>? TodoChanged;
    Task SendNotificationAsync(TodoItem item);
}

public class NotificationService : IAsyncDisposable, INotificationService
{
    public NotificationService(NavigationManager navigationManager)
    {
        _hubConnection = new HubConnectionBuilder()
        .WithUrl(navigationManager.ToAbsoluteUri("/NotificationHub"))
        .Build();
        _hubConnection.On<TodoItem>("TodoChanged", (post) =>
        {
            TodoChanged?.Invoke(post);
        });
        _hubConnection.StartAsync();
    }
    private readonly HubConnection _hubConnection;
    public event Action<TodoItem>? TodoChanged;

    public async Task SendNotificationAsync(TodoItem item)
    {
        await _hubConnection.SendAsync("SendNotification", item);
    }
    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }
}
