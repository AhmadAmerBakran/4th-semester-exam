using Core.Interfaces;

namespace Service;

public class CarControlService : ICarControlService
{
        private readonly IMQTTClientManager _mqttClientManager;
        
        public CarControlService(IMQTTClientManager mqttClientManager)
        {
            _mqttClientManager = mqttClientManager;
            
        }
        
        public void HandleReceivedMessage(string topic, string message)
        {
            OnNotificationReceived?.Invoke(topic, message);
        }
        
        public async Task CarControl(string topic, string command)
        {
            await _mqttClientManager.PublishAsync(topic, command);
        }
        public async Task OpenConnection()
        {
            await _mqttClientManager.ConnectAsync();
        }

        public async Task CloseConnection()
        {
            await _mqttClientManager.DisconnectAsync();
        }

        public async Task GetNotifications()
        {
             _mqttClientManager.InitializeSubscriptions();
             _mqttClientManager.MessageReceived += HandleReceivedMessage;
        }
        
        public event Action<string, string> OnNotificationReceived;
}