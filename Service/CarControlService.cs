using Core.Interfaces;
using MQTTClient;

namespace Service;

public class CarControlService : ICarControlService
{
        private readonly MQTTClientManager _mqttClientManager;

        public CarControlService(MQTTClientManager mqttClientManager)
        {
            _mqttClientManager = mqttClientManager;
            _mqttClientManager.MessageReceived += HandleReceivedMessage;
        }

        public void HandleReceivedMessage(string topic, string message)
        {
            OnNotificationReceived?.Invoke(topic, message);
        }

        public async Task CarControl(string topic, string command)
        {
            await _mqttClientManager.PublishAsync(topic, command);
        }

        public async Task GetNotifications(string topic)
        {
            await _mqttClientManager.SubscribeAsync(topic);
        }

        public event Action<string, string> OnNotificationReceived;
    
}