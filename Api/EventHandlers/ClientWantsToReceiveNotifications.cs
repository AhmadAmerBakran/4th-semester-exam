using System.Text.Json;
using Api.Dtos;
using Api.Filters;
using Api.State;
using Core.Exceptions;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

[RequireAuthentication]
public class ClientWantsToReceiveNotifications : BaseEventHandler<ClientWantsToReceiveNotificationsDto>
{
    private readonly ICarControlService _notificationService;
    private readonly IWebSocketConnectionManager _webSocketConnectionManager;

    public ClientWantsToReceiveNotifications(ICarControlService notificationService, IWebSocketConnectionManager webSocketConnectionManager)
    {
        _notificationService = notificationService;
        _webSocketConnectionManager = webSocketConnectionManager;
        _notificationService.OnNotificationReceived += OnNotificationReceived;
    }

    private void OnNotificationReceived(string topic, string message)
    {
        foreach (var socket in _webSocketConnectionManager.GetAllConnections())
        {
            try
            {
                socket.Connection.Send($"Notification on '{topic}': {message}");
            }
            catch (AppException ex)
            {
                socket.Connection.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
                {
                    ErrorMessage = ex.Message
                }));
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                var errorMessage = "An unexpected error occurred. Please try again later.";
                socket.Connection.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
                {
                    ErrorMessage = errorMessage
                }));
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
        }
    }

    public override async Task Handle(ClientWantsToReceiveNotificationsDto dto, IWebSocketConnection socket)
    {
        await Task.CompletedTask; 
    }
}