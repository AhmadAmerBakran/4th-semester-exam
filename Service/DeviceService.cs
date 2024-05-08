using Core;
using Core.Interfaces;
using MQTTClient;

namespace Service;

public class DeviceService : IDeviceService
{
    private readonly MQTTClientManager _mqttClientManager;

    public DeviceService(MQTTClientManager mqttClientManager)
    {
        _mqttClientManager = mqttClientManager;
    }

    public async Task SendCommandAsync(string command)
    {
        await _mqttClientManager.ConnectAsync();
        await _mqttClientManager.PublishAsync("your/control/topic", command);
    }

    public async Task<string> GetDeviceStatusAsync()
    {
        // Implement status retrieval logic
        return "Status: Running";
    }
}