using api.State;
using Fleck;

namespace Api.State;


public class WebSocketConnectionManager : IWebSocketConnectionManager
{
    private readonly Dictionary<Guid, WebSocketWithMetaData> _connections = new();

    public void AddConnection(Guid id, IWebSocketConnection socket)
    {
        if (!_connections.ContainsKey(id))
        {
            _connections[id] = new WebSocketWithMetaData(socket);
            Console.WriteLine($"New connection added with GUID: {id}");
        }
        else
        {
            Console.WriteLine($"Connection with GUID: {id} already exists. Updating socket reference.");
            _connections[id].Connection = socket;
        }
    }

    public void RemoveConnection(Guid id)
    {
        if (_connections.ContainsKey(id))
        {
            _connections[id].Connection.Close();
            _connections.Remove(id);
            Console.WriteLine($"Connection and associated metadata removed: {id}");
        }
        else
        {
            Console.WriteLine($"Attempted to remove non-existent connection with GUID: {id}");
        }
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
    
    public bool IsAuthenticated(IWebSocketConnection socket)
    {
        if (_connections.TryGetValue(socket.ConnectionInfo.Id, out WebSocketWithMetaData metaData))
        {
            return !string.IsNullOrEmpty(metaData.Username);
        }
        return false;
    }
    
    public bool HasMetadata(Guid id)
    {
        return _connections.ContainsKey(id);
    }
}
