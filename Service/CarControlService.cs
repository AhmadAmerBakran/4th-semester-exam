using System.Collections;
using Core.Exceptions;
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
            try
            {
                _carLogRepository.AddNotificationAsync(_guid, topic, null, message).Wait();
            }
            catch (Exception ex)
            {
                throw new AppException("An error occurred while handling the received message. Please try again later.");
            }
        }
        
        public async Task CarControl(Guid userId, string topic, string command)
        {
            try
            {
                await _mqttClientManager.PublishAsync(topic, command);
                await _carLogRepository.AddNotificationAsync(userId, null, topic, command);
                _guid = userId;
            }
            catch (AppException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException("An unexpected error occurred while controlling the car. Please try again later.");
            }
        }
        public async Task OpenConnection()
        {
            try
            {
                await _mqttClientManager.ConnectAsync();
                _mqttClientManager.InitializeSubscriptions();
                _mqttClientManager.MessageReceived += HandleReceivedMessage;
            }
            catch (AppException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException("An unexpected error occurred while opening the connection. Please try again later.");
            }
        }

        public async Task CloseConnection()
        {
            try
            {
                await _mqttClientManager.DisconnectAsync();
            }
            catch (AppException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException("An unexpected error occurred while closing the connection. Please try again later.");
            }
        }

        public async Task AddUserAsync(Guid userId, string nickname)
        {
            try
            {
                await _carLogRepository.AddUserAsync(userId, nickname);
            }
            catch (AppException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException("An unexpected error occurred while adding the user. Please try again later.");
            }
        }
        
        public event Action<string, string> OnNotificationReceived;

        public async Task <IEnumerable> GetCarLog()
        {
            try
            {
                return await _carLogRepository.GetCarLog();
            }
            catch (AppException ex)
            {
                throw; // Rethrow custom exceptions as is
            }
            catch (Exception ex)
            {
                throw new AppException("An unexpected error occurred while fetching the car log. Please try again later.");
            }
        }
}