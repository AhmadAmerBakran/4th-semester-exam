using api.State;
using Core.Exceptions;
using Fleck;

namespace Api.State;


public class WebSocketConnectionManager : IWebSocketConnectionManager
{
    private readonly Dictionary<Guid, WebSocketWithMetaData> _connections = new();

    public void AddConnection(Guid id, IWebSocketConnection socket)
    {
        try
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
        catch (Exception ex)
        {
            throw new AppException("An error occurred while establishing the connection. Please try again later.");
        }
    }

    public void RemoveConnection(Guid id)
    {
        try
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
        catch (Exception ex)
        {
            throw new AppException("An error occurred while disconnecting. Please try again later or close the app.");
        }
    }

    

    public WebSocketWithMetaData GetConnection(Guid id)
    {
        try
        {
            _connections.TryGetValue(id, out var metaData);
            return metaData;
        }
        catch (Exception ex)
        {
            throw new AppException("An error occurred while retrieving a connection. Please try again later.");
        }
    }

    public IEnumerable<WebSocketWithMetaData> GetAllConnections()
    {
        try
        {
            return _connections.Values;
        }
        catch (Exception ex)
        {
            throw new AppException("An error occurred while retrieving all connections. Please try again later.");
        }
    }
    
    public bool IsAuthenticated(IWebSocketConnection socket)
    {
        try
        {
            if (_connections.TryGetValue(socket.ConnectionInfo.Id, out WebSocketWithMetaData metaData))
            {
                return !string.IsNullOrEmpty(metaData.Username);
            }
            return false;
        }
        catch (Exception ex)
        {
            throw new AppException("An error occurred while checking authentication. Please try again later.");
        }
    }
    
    public bool HasMetadata(Guid id)
    {
        try
        {
            return _connections.ContainsKey(id);
        }
        catch (Exception ex)
        {
            throw new AppException("An error occurred while checking metadata. Please try again later.");
        }
    }
}
