using System;
using System.Diagnostics;
using ManagementBus.Interfaces;
using ManagementBus.Tests.Messages;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ManagementBus.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        private IClient _client;

        [SetUp]
        public void SetupTest()
        {
        }
        
        public void TestMessageHandler(TestMessage message)
        {
            
        }

        [Test]
        public void TestMethod1()
        {
            //var data = new TestMessage
            //{
            //    CircuitName = "this name"
            //};

            //var message = new Message
            //{
            //    Host = "Client",
            //    DateCreated = DateTimeOffset.Now,
            //    DataType = data.GetType().FullName
            //};

            //message.Data = JsonConvert.SerializeObject(data);

            //string outgoingMessage = JsonConvert.SerializeObject(message);

            //Trace.WriteLine(outgoingMessage);
        }
    }
}
