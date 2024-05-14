using Fleck;

namespace Api.Middlewares;

public static class WebSocketMiddlewareExtensions
{
    public static Func<IWebSocketConnection, Task> UseWebSocketExceptionMiddleware(this Func<IWebSocketConnection, Task> next)
    {
        return new WebSocketExceptionMiddleware(next).InvokeAsync;
    }
}
