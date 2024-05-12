using Api.Dtos;
using Api.State;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class ClientWantsToSignOut : BaseEventHandler<ClientWantsToSignOutDto>
{
    private readonly IWebSocketConnectionManager _webSocketConnectionManager;

    public ClientWantsToSignOut(IWebSocketConnectionManager webSocketConnectionManager)
    {
        _webSocketConnectionManager = webSocketConnectionManager;
    }

    public override async Task Handle(ClientWantsToSignOutDto dto, IWebSocketConnection socket)
    {
        try
        {
            _webSocketConnectionManager.RemoveConnection(socket.ConnectionInfo.Id);
            socket.Close();
            Console.WriteLine("User signed out and connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing sign out: {ex.Message}");
            if (socket.IsAvailable)
            {
                socket.Send("Error processing your sign out request.");
            }
        }

        await Task.CompletedTask;
    }
}
