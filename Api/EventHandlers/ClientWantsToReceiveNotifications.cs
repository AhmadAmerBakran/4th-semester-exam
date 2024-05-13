using Api.Dtos;
using Api.State;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class ClientWantsToReceiveNotifications : BaseEventHandler<ClientWantsToReceiveNotificationsDto>
{
    private readonly ICarControlService _notificationService;
    private readonly IWebSocketConnectionManager _webSocketConnectionManager;

    public ClientWantsToReceiveNotifications(ICarControlService notificationService, IWebSocketConnectionManager webSocketConnectionManager)
    {
        _notificationService = notificationService;
        _notificationService.GetNotifications();
        _webSocketConnectionManager = webSocketConnectionManager;
        _notificationService.OnNotificationReceived += OnNotificationReceived;
    }

    private void OnNotificationReceived(string topic, string message)
    {
        foreach (var socket in _webSocketConnectionManager.GetAllConnections())
        {
            socket.Connection.Send($"Notification on '{topic}': {message}");
        }
    }
    public override async Task Handle(ClientWantsToReceiveNotificationsDto dto, IWebSocketConnection socket)
    {
        await Task.CompletedTask; 
    }
}