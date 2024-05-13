using System.Text.Json;
using Api.Dtos;
using Api.State;
using Fleck;
using lib;

namespace Api.Filters;

public class RequireAuthenticationAttribute : BaseEventFilter
{
    
    public override Task Handle<T>(IWebSocketConnection socket, T dto)
    {
        
        var connectionManager = ServiceLocator.ServiceProvider.GetService<IWebSocketConnectionManager>();
        if (connectionManager == null)
        {
            throw new InvalidOperationException("ConnectionManager service is not available.");
        }

        if (!connectionManager.IsAuthenticated(socket))
        {
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
            {
                ErrorMessage = "You must sign in before you connect."
            }));
            throw new UnauthorizedAccessException("Client must be authenticated to use this feature.");
        }

        return Task.CompletedTask;
    }
}

public static class ServiceLocator
{
    public static IServiceProvider ServiceProvider { get; set; }
}