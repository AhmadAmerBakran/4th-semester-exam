using System.Reflection;
using Core.Interfaces;
using Fleck;
using Infrastructure;
using MQTTClient;
using Service;
using Api.State;
using lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString, dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());
builder.Services.AddSingleton<IMQTTClientManager, MQTTClientManager>();
builder.Services.AddSingleton<ICarControlService, CarControlService>();
builder.Services.AddHostedService<MqttBackgroundService>();

var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

var app = builder.Build();

//var port = Environment.GetEnvironmentVariable("PORT") ?? "8181";
var server = new WebSocketServer("ws://0.0.0.0:8181");
server.Start(socket =>
{
    socket.OnOpen = () => {
        Console.WriteLine("Connection opened.");
        WebSocketConnectionManager.AddSocket(socket);
    };
    socket.OnClose = () => {
        Console.WriteLine("Connection closed.");
        WebSocketConnectionManager.RemoveSocket(socket);
    };
    socket.OnMessage =  async message =>
        {
            try
            {
                await app.InvokeClientEventHandler(clientEventHandlers, socket, message);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing command: {e.Message}");
                socket.Send("Error processing your command.");
            }
        };
});

app.Run();
