using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ManagementBus
{
    public class Client : IClient
    {
        private const int ConnectionRetrySeconds = 30;

        private MqttClient _mqttClient;
        private readonly MqttConfiguration _config;
        private List<ITopicSubscription> _topicSubscriptions;

        private System.Timers.Timer _connectionTimer;

        public Client()
        {
            _topicSubscriptions = Enumerable.Empty<ITopicSubscription>().ToList();

            _config = new MqttConfiguration();

            StartConnectionTimer();
            ConnectAndSetupListener();
        }

        public delegate void MessageReceivedEventHandler(string topic, string message);

        public void SendMessage(string topic, string message)
        {
            if (!IsConnected)
                return;

            var byteMessage = Encoding.UTF8.GetBytes(message);

            try
            {
                _mqttClient.Publish(topic, byteMessage, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to send message to topic: {0}.  Exception: {1}", topic, ex);
            }
        }


        private bool _isConnecting;

        private void ConnectAndSetupListener()
        {
            // Validate that we're not trying to establish a connection or are already connected.
            if (_isConnecting || IsConnected)
                return;

            try
            {
                _isConnecting = true;

                // TODO: Add security credentials / make configurable.
                _mqttClient = new MqttClient(_config.Server, _config.ServerPort, false, null);
                _mqttClient.MqttMsgPublishReceived += MqttClientOnMqttMsgPublishReceived;
                _mqttClient.Connect(_config.ClientId);

                SubscribeTopics();
            }
            catch (Exception ex)
            {
                // TODO: Look into logging.
                Trace.TraceError("Failed to connect to server: {0}:{1}.  Exception: {2}",
                    _config == null ? string.Empty : _config.Server,
                    _config == null ? string.Empty : _config.ServerPort.ToString(),
                    ex);
            }
            finally
            {
                _isConnecting = false;
            }
        }


        public List<ITopicSubscription> TopicSubscriptions
        {
            get { return _topicSubscriptions; }
            set
            {
                _topicSubscriptions = value;
                
                // Create the subscriptions.
                SubscribeTopics();
            }
        }

        private void SubscribeTopics()
        {
            if (!IsConnected)
                return;

            foreach (var subscription in _topicSubscriptions)
            {
                try
                {
                    // Subscribing to an array of topics appears to break M2Mqtt.
                    _mqttClient.Subscribe(new[] { subscription.Topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Topic subscription failed.  Topic: {0}  Exception: {1}", 
                        subscription != null && !string.IsNullOrEmpty(subscription.Topic) ? subscription.Topic : string.Empty,
                        ex);
                }
            }
        }

        public string ClientId
        {
            get { return _config.ClientId; }
        }

        public bool IsConnected
        {
            get
            {
                if (_mqttClient != null && _mqttClient.IsConnected)
                    return true;

                return false;
            }
        }

        private void MqttClientOnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var topicSubs = _topicSubscriptions.Where(x => x.Topic == e.Topic);

            foreach (var topicSub in topicSubs)
            {
                // Process new message.
                topicSub.ProcessMessage(e.Message);
            }
        }
        
        private void StartConnectionTimer()
        {
            if (_connectionTimer != null)
                return;

            _connectionTimer = new System.Timers.Timer(ConnectionRetrySeconds*1000);
            _connectionTimer.Elapsed += (sender, args) => ConnectAndSetupListener();
            _connectionTimer.Enabled = true;
            _connectionTimer.Start();
        }
    }
}