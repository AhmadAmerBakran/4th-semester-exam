namespace Core.Interfaces;

public interface ICarControlService
{
    void HandleReceivedMessage(string topic, string message);
    Task CarControl(string topic, string command);
    Task GetNotifications(string topic);
    Task OpenConnection();
    event Action<string, string> OnNotificationReceived;
}