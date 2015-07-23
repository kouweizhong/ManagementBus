namespace ManagementBus
{
    public interface ITopicSubscription
    {
        /// <summary>
        /// Identifier of the topic to subscribe to.
        /// </summary>
        string Topic { get; set; }
        
        /// <summary>
        /// Process the message received on the topic.
        /// </summary>
        /// <param name="message"></param>
        void ProcessMessage(byte[] message);
    }
}