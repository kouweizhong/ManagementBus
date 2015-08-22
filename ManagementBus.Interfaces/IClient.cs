using System;

namespace ManagementBus.Interfaces
{
    public interface IClient
    {
        void SendMessage<T>(string topic, T data) where T : class;
        string ClientId { get; }
        bool IsConnected { get; }
        void AddTopicSubscription<T>(string topic, Action<T> messageHandler) where T : class;
    }
}