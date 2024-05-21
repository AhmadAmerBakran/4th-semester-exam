namespace Core.Interfaces;

public interface IAIService
{
    Task<string> ProcessCommandAsync(string command, string targetLanguage = "en");
    Task<byte[]> ConvertTextToSpeechAsync(string text);
}