using Api.Dtos;
using Api.State;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class NotificationsHandler : BaseEventHandler<NotificationsDto>
    {
        private readonly ICarControlService _notificationService;

        public NotificationsHandler(ICarControlService notificationService)
        {
            _notificationService = notificationService;
            _notificationService.OnNotificationReceived += OnNotificationReceived;
        }

        private void OnNotificationReceived(string topic, string message)
        {
            foreach (var socket in WebSocketConnectionManager.GetAllSockets())
            {
                socket.Send($"Notification on '{topic}': {message}");
            }
        }

        public override async Task Handle(NotificationsDto dto, IWebSocketConnection socket)
        {
            await Task.CompletedTask; // Placeholder for actual logic
        }
    }