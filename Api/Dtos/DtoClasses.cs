using lib;

namespace Api.Dtos;

public class ClientWantsToControlCarDto : BaseDto
{
    public string Topic { get; set; }
    public string Command { get; set; }
}

public class NotificationsHandlerDto : BaseDto
{
    public string Topic { get; set; }
}

public class ServerSendsErrorMessageToClient : BaseDto
{
    public string ErrorMessage { get; set; }
}

public class ClientWantsToSignInDto: BaseDto
{
    public string NickName { get; set; }
}

public class ClientWnatsToSignOutDto : BaseDto
{
    
}

public class ServerClientSignIn : BaseDto
{
    public string Message { get; set; }
}