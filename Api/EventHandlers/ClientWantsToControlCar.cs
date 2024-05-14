using System.Text.Json;
using Api.Dtos;
using Api.Filters;
using Core.Interfaces;
using Fleck;
using lib;

namespace Api.EventHandlers;

[RequireAuthentication]
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
            await _carControlService.CarControl(socket.ConnectionInfo.Id, dto.Topic, dto.Command);
            await socket.Send($"Command '{dto.Command}' sent to topic '{dto.Topic}'.");
        }
        catch (Exception ex)
        {
            socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
            {
                ErrorMessage = "Error in sign-in process."
            }));
            socket.Send($"Failed to send command: {ex.Message}");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException);
            Console.WriteLine(ex);
        }
    }
}