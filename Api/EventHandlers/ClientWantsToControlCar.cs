using System.Text.Json;
using Api.Dtos;
using Api.Filters;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Fleck;
using lib;
using Service.AiService;

namespace Api.EventHandlers;

[RequireAuthentication]
public class ClientWantsToControlCar : BaseEventHandler<ClientWantsToControlCarDto>
{
    private readonly ICarControlService _carControlService;
    private readonly ILogger<ClientWantsToControlCar> _logger;
    private readonly IAIService _aiService;

    public ClientWantsToControlCar(ICarControlService carControlService, ILogger<ClientWantsToControlCar> logger, IAIService aiService)
    {
        _carControlService = carControlService;
        _logger = logger;
        _aiService = aiService;
    }

    public override async Task Handle(ClientWantsToControlCarDto dto, IWebSocketConnection socket)
    {
        try
        {
            _logger.LogInformation("Client {ClientId} requested car control with command {Command} on topic {Topic}.", socket.ConnectionInfo.Id, dto.Command, dto.Topic);

            // Check if the command is in the command map (AI-generated)
            if (CommandMapping.CommandMap.TryGetValue(dto.Command, out int aiMappedCommand))
            {
                await ProcessAICommand(dto, socket, aiMappedCommand);
            }
            else
            {
                // Handle manual command directly
                await _carControlService.CarControl(socket.ConnectionInfo.Id, dto.Topic, dto.Command);
                var successMessage = $"Manual command '{dto.Command}' sent to topic '{dto.Topic}'.";
                _logger.LogInformation(successMessage);
                await socket.Send(successMessage);
            }
        }
        catch (AppException ex)
        {
            var errorMessage = ex.Message;
            _logger.LogError(ex, "An application error occurred while processing the car control request for client {ClientId}.", socket.ConnectionInfo.Id);

            await socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
            {
                ErrorMessage = errorMessage
            }));
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred. Please try again later.";
            _logger.LogError(ex, errorMessage);

            await socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
            {
                ErrorMessage = errorMessage
            }));
        }
    }

    private async Task ProcessAICommand(ClientWantsToControlCarDto dto, IWebSocketConnection socket, int aiMappedCommand)
    {
        try
        {
            var aiResponse = await _aiService.ProcessCommandAsync(dto.Command);

            if (CommandMapping.CommandMap.TryGetValue(aiResponse, out int commandValue))
            {
                await _carControlService.CarControl(socket.ConnectionInfo.Id, dto.Topic, commandValue.ToString());

                var audioData = await _aiService.ConvertTextToSpeechAsync(aiResponse);
                var base64Audio = Convert.ToBase64String(audioData);

                var successMessage = $"AI processed command '{dto.Command}' to '{commandValue}' and sent to topic '{dto.Topic}'.";
                _logger.LogInformation(successMessage);

                await socket.Send(successMessage);
                await socket.Send(base64Audio); // Send the audio data in base64 format
            }
            else
            {
                var errorMessage = $"AI-generated command '{aiResponse}' not recognized.";
                _logger.LogError(errorMessage);
                await socket.Send(errorMessage);
            }
        }
        catch (Exception ex)
        {
            var errorMessage = "An unexpected error occurred while processing the AI command. Please try again later.";
            _logger.LogError(ex, errorMessage);

            await socket.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClientDto
            {
                ErrorMessage = errorMessage
            }));
        }
    }
}
