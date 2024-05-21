namespace Core.Models;

public static class CommandMapping
{
    public static readonly Dictionary<string, int> CommandMap = new Dictionary<string, int>
    {
        { "forward", 1 },
        { "backward", 2 },
        { "left", 5 },
        { "right", 6 },
        /*{ "reverse right", 5 },
        { "reverse left", 6 },*/
        { "auto", 7 },
        { "stop", 0 },
        /*{"flash on", "on"}, 
        {"flash off", "off"}, */
    };
}
