using System;
using Newtonsoft.Json;

namespace ManagementBus
{
    public class TopicSubscription<T> : ITopicSubscription where T : IMessage
    {
        private readonly Action<T> _messageProcessor;

        public TopicSubscription()
        {
        }

        public TopicSubscription(string topic, Action<T> messageProcessor)
        {
            Topic = topic;
            _messageProcessor = messageProcessor;
        }

        public void ProcessMessage(string message)
        {
            var deserialisedObject = JsonConvert.DeserializeObject<T>(message);

            // Shoot the object off to the message handler/processor.
            if (deserialisedObject != null)
                _messageProcessor(deserialisedObject);
        }

        public string Topic { get; set; }
    }
}