using api.State;
using Fleck;

namespace Api.State;

public class WebSocketConnectionManager
{
    private readonly Dictionary<Guid, WebSocketWithMetaData> _connections = new();

    public void AddConnection(Guid id, IWebSocketConnection socket)
    {
        _connections[id] = new WebSocketWithMetaData(socket);
    }

    public void RemoveConnection(Guid id)
    {
        _connections.Remove(id);
    }

    public WebSocketWithMetaData GetConnection(Guid id)
    {
        _connections.TryGetValue(id, out var metaData);
        return metaData;
    }

    public IEnumerable<WebSocketWithMetaData> GetAllConnections()
    {
        return _connections.Values;
    }
}
