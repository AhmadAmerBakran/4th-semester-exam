using Core.Interfaces;

namespace Service;

public class CarControlService : ICarControlService
{
        private readonly IMQTTClientManager _mqttClientManager;

        public CarControlService(IMQTTClientManager mqttClientManager)
        {
            _mqttClientManager = mqttClientManager;
            _mqttClientManager.MessageReceived += HandleReceivedMessage;
        }

        public void HandleReceivedMessage(string topic, string message)
        {
            OnNotificationReceived?.Invoke(topic, message);
        }

        public async Task ConnectToCar()
        {
            _mqttClientManager.ConnectAsync();
        }
        public async Task CarControl(string topic, string command)
        {
            await _mqttClientManager.PublishAsync(topic, command);
        }
        public async Task OpenConnection()
        {
            await _mqttClientManager.ConnectAsync();
        }

        public async Task GetNotifications(string topic)
        {
            await _mqttClientManager.SubscribeAsync(topic);
        }

        public event Action<string, string> OnNotificationReceived;
    
}