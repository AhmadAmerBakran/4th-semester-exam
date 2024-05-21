using System.Text;
using Newtonsoft.Json;

namespace Service.AiService;

public class TranslationService
{
    private readonly HttpClient _httpClient;
    private readonly string _subscriptionKey;
    private readonly string _endpointUrl;
    private readonly string _region;

    public TranslationService()
    {
        _subscriptionKey = Environment.GetEnvironmentVariable("TranslationServiceSubscriptionKey");
        _endpointUrl = Environment.GetEnvironmentVariable("TranslationServiceEndpointUrl");
        _region = "northeurope";
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", _region);
    }

    public async Task<string> TranslateTextAsync(string text, string targetLanguage)
    {
        var requestBody = JsonConvert.SerializeObject(new[] { new { Text = text } });
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_endpointUrl}{targetLanguage}", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

        return result[0]?.translations[0]?.text;
    }
}