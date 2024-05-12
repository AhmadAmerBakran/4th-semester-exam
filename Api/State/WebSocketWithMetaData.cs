using Fleck;

namespace api.State;

public class WebSocketWithMetaData
{

    public IWebSocketConnection Connection { get; set; }
    public string Username { get; set; }

    public WebSocketWithMetaData(IWebSocketConnection connection)
    {
        Connection = connection;
        Username = string.Empty;
    }

}