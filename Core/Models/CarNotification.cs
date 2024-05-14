namespace Core.Models;

public class CarNotification
{
    public int UserId { get; set; }
    public string Topic { get; set; }
    public string Message { get; set; }
    public DateTime MessageAt { get; set; }
}