﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ManagementBus.Interfaces;
using Newtonsoft.Json;
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

        public void SendMessage<T>(string topic, T data) where T : class
        {
            if (!IsConnected)
                return;
            
            try
            {
                var dataSerialised = JsonConvert.SerializeObject(data);

                var byteMessage = Encoding.UTF8.GetBytes(dataSerialised);

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
        
        private void SubscribeTopics()
        {
            if (!IsConnected)
                return;

            _topicSubscriptions.ForEach(x => SubscribeTopic(x.Topic));
        }

        private void SubscribeTopic(string topic)
        {
            try
            {
                // Subscribing to an array of topics appears to break M2Mqtt.
                _mqttClient.Subscribe(new[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            catch (Exception ex)
            {
                Trace.TraceError("Topic subscription failed.  Topic: {0}  Exception: {1}", topic, ex);
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

        public void AddTopicSubscription<T>(string topic, Action<T> messageHandler) where T : class
        {
            _topicSubscriptions.Add(new TopicSubscription<T>(topic, messageHandler));

            if (IsConnected)
                SubscribeTopic(topic);

        }

        private void MqttClientOnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                if (e.Message == null || e.Message.Length == 0)
                    return;

                var topicSubs = _topicSubscriptions.Where(x => x.Topic == e.Topic).ToList();

                if (!topicSubs.Any())
                    return;

                var messageString = Encoding.Default.GetString(e.Message);
                
                foreach (var topicSub in topicSubs)
                {
                    topicSub.ProcessMessage(messageString);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("an error occured while processing published message.  Ex: {0}.  Stack: {1}", ex.Message, ex.StackTrace));
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