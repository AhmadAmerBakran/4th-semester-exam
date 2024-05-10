using lib;

namespace Api.Dtos;

public class ClientWantsToControlCarDto : BaseDto
{
    public string Topic { get; set; }
    public string Command { get; set; }
}

public class NotificationsDto : BaseDto
{
    public string Topic { get; set; }
}

public class ServerSendsErrorMessageToClient : BaseDto
{
    public string ErrorMessage { get; set; }
}