using System.Collections.Generic;

namespace ManagementBus
{
    public interface IClient
    {
        void SendMessage(string topic, string message);
        List<ITopicSubscription> TopicSubscriptions { get; set; }
        string ClientId { get; }
    }
}