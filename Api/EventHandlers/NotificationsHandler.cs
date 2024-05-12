using Api.Dtos;
using Api.State;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class NotificationsHandler : BaseEventHandler<NotificationsHandlerDto>
    {
        private readonly ICarControlService _notificationService;
        private readonly IWebSocketConnectionManager _webSocketConnectionManager;

        public NotificationsHandler(ICarControlService notificationService, IWebSocketConnectionManager webSocketConnectionManager)
        {
            _notificationService = notificationService;
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

        public override async Task Handle(NotificationsHandlerDto dto, IWebSocketConnection socket)
        {
            await Task.CompletedTask; 
        }
    }