using System.Text.Json;
using Api.Dtos;
using Api.State;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    private readonly ICarControlService _carControlService;
    private readonly WebSocketConnectionManager _webSocketConnectionManager;

    public ClientWantsToSignIn(ICarControlService carControlService, WebSocketConnectionManager webSocketConnectionManager)
    {
        _carControlService = carControlService;
        _webSocketConnectionManager = webSocketConnectionManager;
    }

    public override async Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        try
        {
            await _carControlService.OpenConnection();
            var metaData = _webSocketConnectionManager.GetConnection(socket.ConnectionInfo.Id);

            if (metaData == null)
            { 
                socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClient
                {
                    ErrorMessage = "Failed to sign in due to missing connection metadata."
                }));
            }
            
            metaData.Username = dto.NickName;
            socket.Send(JsonSerializer.Serialize(new ServerClientSignIn()
            {
                Message = "You have connected as " + dto.NickName
            }));
        }
        catch (Exception ex)
        {
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClient
            {
                ErrorMessage = "Error in sign-in process."
            }));
        }
    }
}