﻿using System.Security.Authentication;
using System.Text;
using Core.Exceptions;
using Core.Interfaces;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Formatter;
using MQTTnet.Protocol;


namespace MQTTClient
{

    public class MQTTClientManager : IMQTTClientManager
    {
        private IMqttClient _client;
        private MqttFactory _factory;
        public event Action<string, string> MessageReceived;

        public MQTTClientManager()
        {
            _factory = new MqttFactory();
            _client = _factory.CreateMqttClient();
        }
        public void InitializeSubscriptions()
        {
            _client.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;
            try
            {
                SubscribeAsync("car/notifications").Wait();
            }
            catch (AggregateException ex)
            {
                var innerEx = ex.InnerException;
                if (innerEx is MqttCommunicationException || innerEx is TimeoutException || innerEx is AuthenticationException || innerEx is Exception)
                {
                    Console.WriteLine($"Error: {innerEx.Message}");
                    throw new AppException("A MQTT subscription error occurred. Please try again later.");
                }
            }
        }
        private async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            Console.WriteLine($"Received on {topic}: {message}");
            MessageReceived?.Invoke(topic, message);
            await Task.CompletedTask;
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
            catch (MqttCommunicationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("An error occurred while connecting to the MQTT broker. Please try again later.");
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("Authentication failed while connecting to the MQTT broker. Please check your credentials.");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("The connection to the MQTT broker timed out. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("An unexpected error occurred while connecting to the MQTT broker. Please try again later.");
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                await _client.DisconnectAsync();
                Console.WriteLine("Disconnected from MQTT Broker.");
            }
            catch (MqttCommunicationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("An error occurred while disconnecting from the MQTT broker. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("An unexpected error occurred while disconnecting from the MQTT broker. Please try again later.");
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

            try
            {
                await _client.PublishAsync(mqttMessage);
                Console.WriteLine($"Published to {topic}: {message}");
            }
            catch (MqttCommunicationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("An error occurred while publishing to the MQTT broker. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("An unexpected error occurred while publishing to the MQTT broker. Please try again later.");
            }
        }

        public async Task SubscribeAsync(string topic)
        {
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f =>
                    f.WithTopic(topic)
                        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce))
                .Build();

            try
            {
                await _client.SubscribeAsync(subscribeOptions);
                Console.WriteLine($"Subscribed to {topic}");
            }
            catch (MqttCommunicationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("An error occurred while subscribing to the MQTT broker. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new AppException("An unexpected error occurred while subscribing to the MQTT broker. Please try again later.");
            }
        }
    }
}