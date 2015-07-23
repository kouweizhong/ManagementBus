ManagementBus
==========
Light weight application internal management and notification system.

Grown from the need to be able to actively monitor, reset and switch broken circuits in live software systems.

Uses:
* [MQTT](http://mqtt.org/)
* M2MQTT [github](https://github.com/Hades32/m2mqtt), [nuget](https://www.nuget.org/packages/M2Mqtt/)

Tested using:
* [Mosquitto](http://mosquitto.org/)


Future wants:
* Add standard properties to IMessage such as:
  * Host/ClientId
  * Software project name implementing message
* JSON serialise data instead of pipe delimited string.
* Abstract communication provider.
  * Provide lightweight REST server/client
  * I hear [Redis](http://redis.io/) can do pub/sub