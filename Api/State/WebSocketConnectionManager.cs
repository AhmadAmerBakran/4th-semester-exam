using Fleck;

namespace Api.State;

public static class WebSocketConnectionManager
{
    private static readonly List<IWebSocketConnection> Sockets = new List<IWebSocketConnection>();

    public static void AddSocket(IWebSocketConnection socket)
    {
        Sockets.Add(socket);
    }

    public static void RemoveSocket(IWebSocketConnection socket)
    {
        Sockets.Remove(socket);
    }

    public static IEnumerable<IWebSocketConnection> GetAllSockets()
    {
        return Sockets;
    }
}