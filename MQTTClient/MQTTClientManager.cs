using System.Security.Authentication;
using System.Text;
using Core.Interfaces;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;


namespace MQTTClient
{
    public delegate void MqttMessageReceivedHandler(string topic, string message);

    public class MQTTClientManager : IMQTTClientManager
    {
        private IMqttClient _client;
        private MqttFactory _factory;
        public event MqttMessageReceivedHandler MessageReceived;

        public MQTTClientManager()
        {
            _factory = new MqttFactory();
            _client = _factory.CreateMqttClient();
            _client.ApplicationMessageReceivedAsync += HandleReceivedApplicationMessage;

        }

        private Task HandleReceivedApplicationMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            var receivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine($"Received message on {e.ApplicationMessage.Topic}: {receivedMessage}");
            try
            {
                MessageReceived?.Invoke(e.ApplicationMessage.Topic, receivedMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling MQTT message: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        public async Task ConnectAsync()
        {
            if (_client.IsConnected)
            {
                Console.WriteLine("Already connected to MQTT Broker.");
                return;
            }
            var tlsOptions = new MqttClientOptionsBuilderTlsParameters
            {
                UseTls = true,
                AllowUntrustedCertificates = true,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
                SslProtocol = SslProtocols.Tls12
            };

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(MQTTConfig.ClientId)
                .WithTcpServer(MQTTConfig.Server, MQTTConfig.Port)
                .WithCredentials(MQTTConfig.Username, MQTTConfig.Password)
                .WithCleanSession()
                .WithTls(tlsOptions)
                .WithProtocolVersion(MqttProtocolVersion.V500)
                .Build();

            try
            {
                await _client.ConnectAsync(mqttClientOptions, CancellationToken.None);
                Console.WriteLine("Connected to MQTT Broker.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect: {ex.Message}");
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                await _client.DisconnectAsync();
                Console.WriteLine("Disconnected from MQTT Broker.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to disconnect: {ex.Message}");
            }
        }

        public async Task PublishAsync(string topic, string message)
        {
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(message))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();

            await _client.PublishAsync(mqttMessage);
            Console.WriteLine($"Published to {topic}: {message}");
        }

        public async Task SubscribeAsync(string topic)
        {
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f =>
                    f.WithTopic(topic)
                        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce))
                .Build();

            await _client.SubscribeAsync(subscribeOptions);
            Console.WriteLine($"Subscribed to {topic}");
        }
    }
}