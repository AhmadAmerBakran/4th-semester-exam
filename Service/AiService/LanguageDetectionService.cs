using Azure;
using Azure.AI.TextAnalytics;

namespace Service.AiService;

public class LanguageDetectionService
{
    private readonly TextAnalyticsClient _textAnalyticsClient;
    private readonly string endpoint = Environment.GetEnvironmentVariable("TextAnalyticsEndpoint");
    private readonly string azureKey = Environment.GetEnvironmentVariable("TextAnalyticsApiKey");

    public LanguageDetectionService(string endpoint, string apiKey)
    {
        _textAnalyticsClient = new TextAnalyticsClient(new Uri(this.endpoint), new AzureKeyCredential(azureKey));
    }
    
    public async Task<string> DetectLanguageAsync(string text)
    {
        Response<DetectedLanguage> result = await _textAnalyticsClient.DetectLanguageAsync(text);
        return result.Value.Iso6391Name;
    }
}