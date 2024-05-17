using System.Reflection;
using Api.Filters;
using Core.Interfaces;
using Fleck;
using Infrastructure;
using MQTTClient;
using Service;
using Api.State;
using Core.Exceptions;
using Infrastructure.Repositories;
using lib;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddSingleton<IMQTTClientManager, MQTTClientManager>();
builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString, dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());
builder.Services.AddSingleton<ICarControlService, CarControlService>();
builder.Services.AddSingleton<IWebSocketConnectionManager, WebSocketConnectionManager>();
builder.Services.AddSingleton<ICarLogRepository, CarLogRepository>();

var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

var app = builder.Build();

var connectionManager = app.Services.GetRequiredService<IWebSocketConnectionManager>();

var logger = app.Services.GetRequiredService<ILogger<Program>>();


var server = new WebSocketServer("ws://0.0.0.0:8181");


ServiceLocator.ServiceProvider = app.Services;

server.Start(socket =>
{
    socket.OnOpen = () =>
    {
        try
        {
            var connecionPool = connectionManager.GetAllConnections();
            if (connecionPool.Count() <= 1)
            {
                connectionManager.AddConnection(socket.ConnectionInfo.Id, socket);
            }
            else
            {
                socket.Send("The car is in use right now, please try again later");
                socket.Close();
            }
        }
        catch (AppException ex)
        {
            socket.Send(ex.Message);
            logger.LogError(ex, "AppException in OnOpen");

        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred during the connection process. Please try again later.";
            socket.Send(errorMessage);
            Console.WriteLine($"Exception: {ex.Message}");
            logger.LogError(ex, errorMessage);
        }
    };

    socket.OnClose = () =>
    {
        try
        {
            logger.LogInformation("Connection closed.");
            Console.WriteLine("Connection closed.");
            connectionManager.RemoveConnection(socket.ConnectionInfo.Id);
            if (!connectionManager.HasMetadata(socket.ConnectionInfo.Id))
            {
                logger.LogInformation($"Metadata successfully removed for GUID: {socket.ConnectionInfo.Id}");

                Console.WriteLine($"Metadata successfully removed for GUID: {socket.ConnectionInfo.Id}");
            }
            else
            {
                logger.LogWarning($"Failed to remove metadata for GUID: {socket.ConnectionInfo.Id}");

                Console.WriteLine($"Failed to remove metadata for GUID: {socket.ConnectionInfo.Id}");
            }
        }
        catch (AppException ex)
        {
            socket.Send(ex.Message);
            logger.LogError(ex, "AppException in OnClose");

            Console.WriteLine($"AppException: {ex.Message}");
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred while closing the connection. Please try again later.";
            socket.Send(errorMessage);
            Console.WriteLine($"Exception: {ex.Message}");
            logger.LogError(ex, errorMessage);

        }
    };

    socket.OnBinary = (data) =>
    {
        try
        {
            var connections = connectionManager.GetAllConnections();
            foreach (var conn in connections) 
            {
                conn.Connection.Send(data); 
                logger.LogInformation($"Sent frame with length: {data.Length} to connection");

                Console.WriteLine($"Sent frame with length: {data.Length} to connection");
            }
        }
        catch (AppException ex)
        {
            socket.Send(ex.Message);
            logger.LogError(ex, "AppException in OnBinary");

            Console.WriteLine($"AppException: {ex.Message}");
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred while processing binary data. Please try again later.";
            socket.Send(errorMessage);
            Console.WriteLine($"Exception: {ex.Message}");
            logger.LogError(ex, errorMessage);

        }
    };

    socket.OnMessage = async message =>
    {
        try
        {
            var metaData = connectionManager.GetConnection(socket.ConnectionInfo.Id);
            if (metaData != null)
            {
                await app.InvokeClientEventHandler(clientEventHandlers, socket, message);
            }
            else
            {
                socket.Send("No valid session found.");
                logger.LogWarning("No valid session found for socket ID: {SocketId}", socket.ConnectionInfo.Id);

            }
        }
        catch (AppException ex)
        {
            socket.Send(ex.Message);
            logger.LogError(ex, "AppException in OnMessage");

            Console.WriteLine($"AppException: {ex.Message}");
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred while processing the message. Please try again later.";
            socket.Send(errorMessage);
            logger.LogError(ex, errorMessage);

            Console.WriteLine($"Exception: {ex.Message}");
        }
    };
});

app.Run();