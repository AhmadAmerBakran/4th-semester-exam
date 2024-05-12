using api.State;
using Fleck;

namespace Api.State;

public interface IWebSocketConnectionManager
{
    void AddConnection(Guid id, IWebSocketConnection socket);
    void RemoveConnection(Guid id);
    WebSocketWithMetaData GetConnection(Guid id);
    IEnumerable<WebSocketWithMetaData> GetAllConnections();
    bool IsAuthenticated(IWebSocketConnection socket);
    bool HasMetadata(Guid id);
}