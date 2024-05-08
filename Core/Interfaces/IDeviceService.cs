namespace Core.Interfaces;

public interface IDeviceService
{
    Task SendCommandAsync(string command);
    Task<string> GetDeviceStatusAsync();
}
