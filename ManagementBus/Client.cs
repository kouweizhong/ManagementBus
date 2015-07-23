using System.Collections.Generic;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ManagementBus
{
    public class Client : IClient
    {
        private MqttClient _mqttClient;
        private readonly MqttConfiguration _config;
        private List<ITopicSubscription> _topicSubscriptions;

        public Client()
        {
            _config = new MqttConfiguration();
            ConnectAndSetupListener();
        }

        public delegate void MessageReceivedEventHandler(string topic, string message);

        public void SendMessage(string topic, string message)
        {
            if (!IsConnected())
                return;

            var byteMessage = Encoding.UTF8.GetBytes(message);

            _mqttClient.Publish(topic, byteMessage, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        private void ConnectAndSetupListener()
        {
            if (IsConnected())
                return;

            // TODO: Add security credentials / make configurable.
            _mqttClient = new MqttClient(_config.Server, _config.ServerPort, false, null);
            _mqttClient.MqttMsgPublishReceived += MqttClientOnMqttMsgPublishReceived;
            _mqttClient.Connect(_config.ClientId); // TODO: Fail gracefully.
        }


        public List<ITopicSubscription> TopicSubscriptions
        {
            get { return _topicSubscriptions; }
            set
            {
                // TODO: store if not connected and then subscribe when connection established.

                foreach (var subscription in value)
                {
                    // Subscribing to an array of topics appears to break M2Mqtt.
                    _mqttClient.Subscribe(new []{ subscription.Topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });                    
                }

                _topicSubscriptions = value;
            }
        }

        public string ClientId
        {
            get { return _config.ClientId; }
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


        private bool IsConnected()
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
                return true;

            return false;
        }
    }
}