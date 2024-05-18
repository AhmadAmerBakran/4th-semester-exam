using System.Text.Json;
using Api.Dtos;
using Api.State;
using Core.Exceptions;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class ClientWantsToSignOut : BaseEventHandler<ClientWantsToSignOutDto>
{
    private readonly IWebSocketConnectionManager _webSocketConnectionManager;
    private readonly ILogger<ClientWantsToSignOut> _logger;

    public ClientWantsToSignOut(IWebSocketConnectionManager webSocketConnectionManager, ILogger<ClientWantsToSignOut> logger)
    {
        _webSocketConnectionManager = webSocketConnectionManager;
        _logger = logger;
    }

    public override async Task Handle(ClientWantsToSignOutDto dto, IWebSocketConnection socket)
    {
        try
        {
            var connectionId = socket.ConnectionInfo.Id;
            _logger.LogInformation("User with ID {ConnectionId} is signing out.", connectionId);
            
            _webSocketConnectionManager.ResetConnection(connectionId); // Reset metadata
            _webSocketConnectionManager.StartDisconnectTimer(connectionId, () =>
            {
                _logger.LogInformation("Disconnect timer started for user with ID {ConnectionId}.", connectionId);
                _webSocketConnectionManager.RemoveConnection(connectionId);
                socket.Close();
                _logger.LogInformation("User with ID {ConnectionId} connection closed.", connectionId);
            }, 30000);

            _logger.LogInformation("User signed out, but connection remains open for 30 seconds.");
        }
        catch (AppException ex)
        {
            _logger.LogError(ex, "AppException occurred while signing out user with ID {ConnectionId}.", socket.ConnectionInfo.Id);
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
            {
                ErrorMessage = ex.Message
            }));
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred. Please try again later.";
            _logger.LogError(ex, "Unexpected error occurred while signing out user with ID {ConnectionId}.", socket.ConnectionInfo.Id);
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
            {
                ErrorMessage = errorMessage
            }));
        }

        await Task.CompletedTask;
    }
}
