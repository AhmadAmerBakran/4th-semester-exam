using Microsoft.Extensions.Hosting;


namespace MQTTClient
{
    public class MqttBackgroundService : BackgroundService
    {
        private readonly MQTTClientManager _mqttClient;

        public MqttBackgroundService(MQTTClientManager mqttClient)
        {
            _mqttClient = mqttClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(async () => await _mqttClient.DisconnectAsync());

            try
            {
                await _mqttClient.ConnectAsync();
                await _mqttClient.SubscribeAsync("cam/control");
                await _mqttClient.PublishAsync("cam/control", "start");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}