using System.Reflection;
using api.State;
using Core.Interfaces;
using Fleck;
using Infrastructure;
using MQTTClient;
using Service;
using Api.State;
using lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMQTTClientManager, MQTTClientManager>();
builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString, dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());
builder.Services.AddSingleton<ICarControlService, CarControlService>();
builder.Services.AddSingleton<WebSocketConnectionManager>();

var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

var app = builder.Build();

var connectionManager = app.Services.GetRequiredService<WebSocketConnectionManager>();

var server = new WebSocketServer("ws://0.0.0.0:8181");
server.Start(socket =>
{
    socket.OnOpen = () =>
    {
        connectionManager.AddConnection(socket.ConnectionInfo.Id, socket);
    };

    socket.OnClose = () =>
    {
        connectionManager.RemoveConnection(socket.ConnectionInfo.Id);
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
        }catch (Exception e)
        {
                Console.WriteLine($"Error processing command: {e.Message}");
                socket.Send("Error processing your command.");
        }
    };
});

app.Run();
