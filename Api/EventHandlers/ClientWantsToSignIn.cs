using System.Text.Json;
using Api.Dtos;
using Api.Filters;
using Api.State;
using Core.Exceptions;
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
                socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
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
            _carControlService.AddUserAsync(socket.ConnectionInfo.Id, dto.NickName);
        }
        catch (AppException ex)
        {
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
            {
                ErrorMessage = ex.Message
            }));
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred. Please try again later.";
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
            {
                ErrorMessage = errorMessage
            }));
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);
        }
    }
}