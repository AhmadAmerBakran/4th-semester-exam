﻿using System.Text.Json;
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
    private readonly ICarControlService _carControlService;

    public ClientWantsToSignOut(IWebSocketConnectionManager webSocketConnectionManager, ICarControlService carControlService)
    {
        _webSocketConnectionManager = webSocketConnectionManager;
        _carControlService = carControlService;
    }

    public override async Task Handle(ClientWantsToSignOutDto dto, IWebSocketConnection socket)
    {
        try
        {
            var connectionId = socket.ConnectionInfo.Id;
            _webSocketConnectionManager.ResetConnection(connectionId); // Reset metadata
            await _carControlService.CloseConnection();
            /*socket.Send(JsonSerializer.Serialize(new ServerClientSignOut
            {
                Message = "You have signed out successfully."
            }));*/
            Console.WriteLine("User signed out, but connection remains open.");
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

        await Task.CompletedTask;
    }
}
