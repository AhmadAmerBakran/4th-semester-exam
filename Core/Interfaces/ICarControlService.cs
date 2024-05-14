
namespace Core.Interfaces;

public interface ICarControlService
{
    Task CarControl(Guid userId, string topic, string command);
    Task GetNotifications();
    Task OpenConnection();
    Task CloseConnection();
    event Action<string, string> OnNotificationReceived;
    Task AddUserAsync(Guid userId, string nickname);
}