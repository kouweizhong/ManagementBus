using System.Configuration;

namespace ManagementBus
{
    internal class MqttConfiguration
    {
        public MqttConfiguration()
        {
            Server = ConfigurationManager.AppSettings["Client.Mqtt.Server"];

            ServerPort = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Client.Mqtt.ServerPort"]) 
                ? int.Parse(ConfigurationManager.AppSettings["Client.Mqtt.ServerPort"]) 
                : 1883;

            ClientId = ConfigurationManager.AppSettings["Client.Mqtt.ClientId"];
        }

        public string Server { get; set; }
        public int ServerPort { get; set; }
        public string ClientId { get; set; }
    }
}