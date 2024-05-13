using Api.Dtos;
using Api.State;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class ClientWantsToSignOut : BaseEventHandler<ClientWantsToSignOutDto>
{
    private readonly IWebSocketConnectionManager _webSocketConnectionManager;
    private readonly ICarControlService _carControlService;

    public ClientWantsToSignOut(IWebSocketConnectionManager webSocketConnectionManager, ICarControlService carControlService)
    {
        _webSocketConnectionManager = webSocketConnectionManager;
        _carControlService = carControlService;
    }

    public override async Task Handle(ClientWantsToSignOutDto dto, IWebSocketConnection socket)
    {
        try
        {
            _webSocketConnectionManager.RemoveConnection(socket.ConnectionInfo.Id);
            _carControlService.CloseConnection().Wait();
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
