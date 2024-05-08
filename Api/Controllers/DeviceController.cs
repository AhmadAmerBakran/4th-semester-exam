using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpPost("sendCommand")]
    public async Task<IActionResult> SendCommand([FromBody] string command)
    {
        await _deviceService.SendCommandAsync(command);
        return Ok("Command sent successfully");
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var status = await _deviceService.GetDeviceStatusAsync();
        return Ok(status);
    }
}