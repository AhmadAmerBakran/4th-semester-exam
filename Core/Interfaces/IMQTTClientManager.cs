namespace Core.Interfaces;

public interface IMQTTClientManager
{
    event Action<string, string> MessageReceived;  // Declare the event

    Task ConnectAsync();
    Task DisconnectAsync();
    Task PublishAsync(string topic, string message);
    Task SubscribeAsync(string topic);
}