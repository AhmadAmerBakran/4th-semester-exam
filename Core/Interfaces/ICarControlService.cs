namespace Core.Interfaces;

public interface ICarControlService
{
    Task CarControl(string topic, string command);
    Task GetNotifications();
    Task OpenConnection();
    Task CloseConnection();
    event Action<string, string> OnNotificationReceived;
}