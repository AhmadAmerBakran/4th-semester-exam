using Api.Dtos;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

public class ClientWantsToControlCar : BaseEventHandler<ClientWantsToControlCarDto>
{
    private readonly ICarControlService _carControlService;

    public ClientWantsToControlCar(ICarControlService carControlService)
    {
        _carControlService = carControlService;
    }

    public override async Task Handle(ClientWantsToControlCarDto dto, IWebSocketConnection socket)
    {
        try
        {
            // Use the service to send command to MQTT topic
            await _carControlService.CarControl(dto.Topic, dto.Command);
            socket.Send($"Command '{dto.Command}' sent to topic '{dto.Topic}'.");
        }
        catch (Exception ex)
        {
            socket.Send($"Failed to send command: {ex.Message}");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException);
        }
    }
}