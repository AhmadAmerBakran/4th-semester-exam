namespace Core.Interfaces;

public interface IMQTTClientManager
{
    Task ConnectAsync();
    Task DisconnectAsync();
    Task PublishAsync(string topic, string message);
    Task SubscribeAsync(string topic);
}