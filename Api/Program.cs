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

// Add services to the container.
builder.Services.AddSingleton<IMQTTClientManager, MQTTClientManager>();
builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString, dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());
builder.Services.AddSingleton<ICarControlService, CarControlService>();
builder.Services.AddSingleton<IWebSocketConnectionManager, WebSocketConnectionManager>();
builder.Services.AddSingleton<ICarLogRepository, CarLogRepository>();

var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

var app = builder.Build();

var connectionManager = app.Services.GetRequiredService<IWebSocketConnectionManager>();

var server = new WebSocketServer("ws://0.0.0.0:8181");


ServiceLocator.ServiceProvider = app.Services;

server.Start(socket =>
{
    socket.OnOpen = () =>
    {
        try
        {
            var connecionPool = connectionManager.GetAllConnections();
            if (connecionPool.Count() <= 5)
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
            Console.WriteLine($"AppException: {ex.Message}");
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred during the connection process. Please try again later.";
            socket.Send(errorMessage);
            Console.WriteLine($"Exception: {ex.Message}");
        }
    };

    socket.OnClose = () =>
    {
        try
        {
            Console.WriteLine("Connection closed.");
            connectionManager.RemoveConnection(socket.ConnectionInfo.Id);
            if (!connectionManager.HasMetadata(socket.ConnectionInfo.Id))
            {
                Console.WriteLine($"Metadata successfully removed for GUID: {socket.ConnectionInfo.Id}");
            }
            else
            {
                Console.WriteLine($"Failed to remove metadata for GUID: {socket.ConnectionInfo.Id}");
            }
        }
        catch (AppException ex)
        {
            socket.Send(ex.Message);
            Console.WriteLine($"AppException: {ex.Message}");
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred while closing the connection. Please try again later.";
            socket.Send(errorMessage);
            Console.WriteLine($"Exception: {ex.Message}");
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
                Console.WriteLine($"Sent frame with length: {data.Length} to connection");
            }
        }
        catch (AppException ex)
        {
            socket.Send(ex.Message);
            Console.WriteLine($"AppException: {ex.Message}");
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred while processing binary data. Please try again later.";
            socket.Send(errorMessage);
            Console.WriteLine($"Exception: {ex.Message}");
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
            }
        }
        catch (AppException ex)
        {
            socket.Send(ex.Message);
            Console.WriteLine($"AppException: {ex.Message}");
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred while processing the message. Please try again later.";
            socket.Send(errorMessage);
            Console.WriteLine($"Exception: {ex.Message}");
        }
    };
});

app.Run();