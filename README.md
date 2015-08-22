## ManagementBus ##

Light weight application internal management and notification system.

Grown from the need to be able to actively monitor, reset and switch broken [circuits](https://github.com/asherw/ManagedCircuitBreaker) in live software systems.

### Setup ###
Any applications using the ManagementBus will require the following in the appSettings.

    <appSettings>
      <add key="Client.Mqtt.Server" value="localhost" />
      <add key="Client.Mqtt.ServerPort" value="1883" />
      <add key="Client.Mqtt.ClientId" value="MyFancyApp" />
    </appSettings>

### Dependencies ###
- [MQTT](http://mqtt.org/)
- M2MQTT [github](https://github.com/Hades32/m2mqtt), [nuget](https://www.nuget.org/packages/M2Mqtt/)

### MQTT servers tested ###
- [Mosquitto](http://mosquitto.org/)


### Projects implementing management bus ###

- [ManagedCircuitBreaker](https://github.com/asherw/ManagedCircuitBreaker)


### Future wants ###
- Automatic addition of properties to messages such as:
  - Host/ClientId
  - Software project name implementing message
  - Implementing projects version
- Abstract communication provider.
  - Provide lightweight REST server/client
  - I hear [Redis](http://redis.io/) can do pub/sub

