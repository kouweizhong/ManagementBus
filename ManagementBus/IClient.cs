using System.Collections.Generic;

namespace ManagementBus
{
    public interface IClient
    {
        void SendMessage<T>(string topic, T data) where T : class;
        List<ITopicSubscription> TopicSubscriptions { get; set; }
        string ClientId { get; }
        bool IsConnected { get; }
    }
}