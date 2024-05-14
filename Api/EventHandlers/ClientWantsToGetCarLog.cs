using System.Text.Json;
using Api.Dtos;
using Api.Filters;
using Core.Exceptions;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

[RequireAuthentication]
public class ClientWantsToGetCarLog : BaseEventHandler<ClientWantsToGetCarLogDto>
{
    private readonly ICarControlService _carControlService;

    public ClientWantsToGetCarLog(ICarControlService carControlService)
    {
        _carControlService = carControlService;
    }

    public override Task Handle(ClientWantsToGetCarLogDto dto, IWebSocketConnection socket)
    {
        try
        {
            var notifications = _carControlService.GetCarLog().Result;
            foreach (var not in notifications)
            {
                socket.Send(JsonSerializer.Serialize(not));
            }
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
        return Task.CompletedTask;
    }
}