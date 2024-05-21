using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Service.AiService;

public class AIService : IAIService
{
        private readonly TranslationService _translationService;
        private readonly LanguageDetectionService _languageDetectionService;
        private readonly TextToSpeechService _textToSpeechService;
        private readonly ILogger<AIService> _logger;

        public AIService(TranslationService translationService, LanguageDetectionService languageDetectionService, TextToSpeechService textToSpeechService, ILogger<AIService> logger)
        {
            _translationService = translationService;
            _languageDetectionService = languageDetectionService;
            _textToSpeechService = textToSpeechService;
            _logger = logger;
        }

        public async Task<string> ProcessCommandAsync(string command, string targetLanguage = "en")
        {
            try
            {
                // Detect the language of the command
                var detectedLanguage = await _languageDetectionService.DetectLanguageAsync(command);

                // Translate the command if necessary
                if (detectedLanguage != targetLanguage)
                {
                    command = await _translationService.TranslateTextAsync(command, targetLanguage);
                }

                _logger.LogInformation("Processed command: {Command}", command);
                return command;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing command: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<byte[]> ConvertTextToSpeechAsync(string text)
        {
            try
            {
                var audioData = await _textToSpeechService.ConvertTextToSpeechAsync(text);
                _logger.LogInformation("Converted text to speech for text: {Text}", text);
                return audioData;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error converting text to speech: {Message}", ex.Message);
                throw;
            }
        }
        
}