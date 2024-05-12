using System.Text.Json;
using Api.Dtos;
using Api.Filters;
using Api.State;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

[ValidateDataAnnotations]
public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    private readonly ICarControlService _carControlService;
    private readonly IWebSocketConnectionManager _webSocketConnectionManager;

    public ClientWantsToSignIn(ICarControlService carControlService, IWebSocketConnectionManager webSocketConnectionManager)
    {
        _carControlService = carControlService;
        _webSocketConnectionManager = webSocketConnectionManager;
    }

    public override async Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        try
        {
            var metaData = _webSocketConnectionManager.GetConnection(socket.ConnectionInfo.Id);

            if (metaData == null)
            { 
                socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClient
                {
                    ErrorMessage = "Failed to sign in due to missing connection metadata."
                }));
                return;
            }
            await _carControlService.OpenConnection();

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