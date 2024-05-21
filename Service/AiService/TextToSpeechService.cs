using Microsoft.CognitiveServices.Speech;

namespace Service.AiService;

public class TextToSpeechService
{
    private readonly string _subscriptionKey;
    private readonly string _serviceRegion;
    private readonly SpeechConfig _speechConfig;
    private readonly LanguageDetectionService _languageDetectionService;


    public TextToSpeechService(string subscriptionKey, string serviceRegion, LanguageDetectionService languageDetectionService)
    {
        _subscriptionKey = Environment.GetEnvironmentVariable("TextToSpeech:SubscriptionKey");
        _serviceRegion = "northeurope";
        _speechConfig = SpeechConfig.FromSubscription(_subscriptionKey, _serviceRegion);
        _languageDetectionService = languageDetectionService;
    }
    
    public async Task<byte[]> ConvertTextToSpeechAsync(string text)
    {
        string languageCode = await _languageDetectionService.DetectLanguageAsync(text);
        
        string voiceName = GetVoiceNameFromLanguageCode(languageCode);
        
        _speechConfig.SpeechSynthesisVoiceName = voiceName;
        
        using var synthesizer = new SpeechSynthesizer(_speechConfig, null);
        var result = await synthesizer.SpeakTextAsync(text);
        
        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            return result.AudioData;
        }
        else
        {
            throw new InvalidOperationException($"Speech synthesis failed for text: {text}, Reason: {result.Reason}");
        }
    }
    
    private string GetVoiceNameFromLanguageCode(string languageCode)
    {
        var voiceMap = new Dictionary<string, string>
        {
            { "en", "en-US-JennyNeural" },
            { "ar", "ar-LB-LaylaNeural" },
            { "da", "da-DK-ChristelNeural" },
            { "es", "es-ES-AlvaroNeural" },
            { "fr", "fr-FR-DeniseNeural" },
            { "de", "de-DE-KatjaNeural" },
            { "it", "it-IT-ElsaNeural" },
            { "ja", "ja-JP-NanamiNeural" },
            { "pt", "pt-BR-FranciscaNeural" },
            { "zh", "zh-CN-XiaoxiaoNeural" },
            { "hi", "hi-IN-SwaraNeural" }, 
        };

        return voiceMap.TryGetValue(languageCode, out var voiceName) ? voiceName : "en-US-JennyNeural";
    }
    
}