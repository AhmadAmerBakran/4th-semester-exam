using Fleck;

namespace api.State;

public class WebSocketWithMetaData
{
    public IWebSocketConnection Connection { get; set; }
    public string Username { get; set; }
    public Timer DisconnectTimer { get; set; }

    public WebSocketWithMetaData(IWebSocketConnection connection)
    {
        Connection = connection;
        Username = string.Empty;
        DisconnectTimer = null;
    }

    public void StartDisconnectTimer(Action disconnectAction, int timeoutMilliseconds)
    {
        DisconnectTimer?.Dispose();
        DisconnectTimer = new Timer(_ => disconnectAction(), null, timeoutMilliseconds, Timeout.Infinite);
    }

    public void StopDisconnectTimer()
    {
        DisconnectTimer?.Dispose();
        DisconnectTimer = null;
    }
}