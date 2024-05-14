using System.Collections;
using Core.Interfaces;
using Core.Models;

namespace Service;

public class CarControlService : ICarControlService
{
        private readonly IMQTTClientManager _mqttClientManager;
        private readonly ICarLogRepository _carLogRepository;
        public CarControlService(IMQTTClientManager mqttClientManager, ICarLogRepository carLogRepository)
        {
            _mqttClientManager = mqttClientManager;
            _carLogRepository = carLogRepository;
            
        }

        private Guid _guid;
        
        public void HandleReceivedMessage(string topic, string message)
        {
            OnNotificationReceived?.Invoke(topic, message);
            _carLogRepository.AddNotificationAsync(_guid, topic, null, message);
        }
        
        public async Task CarControl(Guid userId, string topic, string command)
        {
            await _mqttClientManager.PublishAsync(topic, command);
            await _carLogRepository.AddNotificationAsync(userId, null, topic, command);
            _guid = userId;
        }
        public async Task OpenConnection()
        {
            await _mqttClientManager.ConnectAsync();
            _mqttClientManager.InitializeSubscriptions();
            _mqttClientManager.MessageReceived += HandleReceivedMessage;

        }

        public async Task CloseConnection()
        {
            await _mqttClientManager.DisconnectAsync();
        }

        public async Task AddUserAsync(Guid userId, string nickname)
        {
            await _carLogRepository.AddUserAsync(userId, nickname);
        }
        
        public event Action<string, string> OnNotificationReceived;

        public async Task <IEnumerable> GetCarLog()
        {
             return await _carLogRepository.GetCarLog();
        }
}