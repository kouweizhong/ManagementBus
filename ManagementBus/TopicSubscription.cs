using System;

namespace ManagementBus
{
    public class TopicSubscription<T> : ITopicSubscription where T : IMessage
    {
        private readonly Action<T> _messageProcessor;
        private readonly Func<byte[], T> _msgConverter;
        private readonly Action<byte[]> _messageRawProcessor;

        public TopicSubscription()
        {
        }

        public TopicSubscription(string topic, Func<byte[], T> msgConverter, Action<T> messageProcessor)
        {
            Topic = topic;
            _msgConverter = msgConverter;
            _messageProcessor = messageProcessor;
        }

        public TopicSubscription(string topic, Action<byte[]> messageProcessor)
        {
            _messageRawProcessor = messageProcessor;
            Topic = topic;
        }

        public void ProcessMessage(byte[] message)
        {
            if (message == null || message.Length == 0)
                return;

            if (_messageProcessor != null)
            {
                // Deserialise the message using the supplied converting method.
                var deserialisedObject = _msgConverter(message);

                // Shoot the object off to the message handler/processor.
                if (deserialisedObject != null)
                    _messageProcessor(deserialisedObject);
            }

            if (_messageRawProcessor != null)
                _messageRawProcessor(message);
        }

        public string Topic { get; set; }
    }
}