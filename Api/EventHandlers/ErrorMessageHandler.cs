using Api.Dtos;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class ErrorMessageHandler : BaseEventHandler<ServerSendsErrorMessageToClient>
{
    public override async Task Handle(ServerSendsErrorMessageToClient dto, IWebSocketConnection socket)
    {
        socket.Send($"Error: {dto.ErrorMessage}");
        await Task.CompletedTask;
    }
}