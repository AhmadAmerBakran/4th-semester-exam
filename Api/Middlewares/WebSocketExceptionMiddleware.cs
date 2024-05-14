using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Net.Sockets;
using Fleck;
using MQTTnet.Exceptions;
using Npgsql;

namespace Api.Middlewares;

public class WebSocketExceptionMiddleware
{
    private readonly Func<IWebSocketConnection, Task> _next;

    public WebSocketExceptionMiddleware(Func<IWebSocketConnection, Task> next)
    {
        _next = next;
    }

    public async Task InvokeAsync(IWebSocketConnection socket)
    {
        try
        {
            await _next(socket);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Unauthorized access: {ex.Message}");
            if (socket.IsAvailable)
            {
                await socket.Send("You are not authorized to perform this action. Please sign in.");
            }
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
            if (socket.IsAvailable)
            {
                await socket.Send("Invalid data provided. Please check your inputs and try again.");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Invalid operation: {ex.Message}");
            if (socket.IsAvailable)
            {
                await socket.Send("An unexpected error occurred. Please try again later.");
            }
        }
        catch (MqttCommunicationException ex)
        {
            Console.WriteLine($"MQTT communication error: {ex.Message}");
            if (socket.IsAvailable)
            {
                await socket.Send("An error occurred while communicating with the MQTT broker. Please try again later.");
            }
        }
        catch (PostgresException ex)
        {
            Console.WriteLine($"PostgreSQL error: {ex.Message}");
            if (socket.IsAvailable)
            {
                await socket.Send("A database error occurred. Please try again later.");
            }
        }
        catch (DbException ex)
        {
            Console.WriteLine($"Database error: {ex.Message}");
            if (socket.IsAvailable)
            {
                await socket.Send("A database error occurred. Please try again later.");
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Socket error: {ex.Message}");
            if (socket.IsAvailable)
            {
                await socket.Send("A network error occurred. Please check your connection and try again.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            if (socket.IsAvailable)
            {
                await socket.Send("An error occurred while processing your request. Please try again later.");
            }
        }
    }
}