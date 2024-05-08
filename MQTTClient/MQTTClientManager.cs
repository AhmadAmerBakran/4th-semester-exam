using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;


namespace MQTTClient
{
    public class MQTTClientManager
    {
        private readonly IMqttClient _client;
        private readonly IMqttClientOptions _options;

        public MQTTClientManager(string server, int port, string clientId, string username, string password)
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();
            _options = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(server, port)
                .WithCredentials(username, password)
                .WithCleanSession()
                .Build();
        }

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync(_options);
        }

        public async Task PublishAsync(string topic, string message)
        {
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(message))
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
        
            await _client.PublishAsync(mqttMessage);
        }
    }
}