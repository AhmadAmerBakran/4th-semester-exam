/*using System.Text.Json;
using Api.Dtos;
using Fleck;
using lib;

namespace Api.Filters;

public class RequireAuthenticationAttribute : BaseEventFilter
{
    public override Task Handle<T>(IWebSocketConnection socket, T dto)
    {
        // Retrieve the IConnectionManager from the ServiceLocator (or directly if available)
        var connectionManager = ServiceLocator.ServiceProvider.GetService<IConnectionManager>();
        if (connectionManager == null)
        {
            throw new InvalidOperationException("ConnectionManager service is not available.");
        }

        // Perform the authentication check using the connectionManager
        if (!connectionManager.IsAuthenticated(socket))
        {
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClient
            {
                ErrorMessage = "You must sign in before entering a room."
            }));
            throw new UnauthorizedAccessException("Client must be authenticated to use this feature.");
        }

        return Task.CompletedTask;
    }
}

public static class ServiceLocator
{
    public static IServiceProvider ServiceProvider { get; set; }
}*/