using System.Text.Json;
using Api.Dtos;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class ClientWantsToGetCarLog : BaseEventHandler<ClientWantsToGetCarLogDto>
{
    private readonly ICarControlService _carControlService;

    public ClientWantsToGetCarLog(ICarControlService carControlService)
    {
        _carControlService = carControlService;
    }

    public override Task Handle(ClientWantsToGetCarLogDto dto, IWebSocketConnection socket)
    {
        var notifications = _carControlService.GetCarLog().Result;
        foreach (var not in notifications)
        {
            socket.Send(JsonSerializer.Serialize(not));
        }
        return Task.CompletedTask;
    }
    
    
}