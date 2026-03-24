const { Client, Message } = require('azure-iot-device');
const { Mqtt } = require('azure-iot-device-mqtt');

const connectionString = 'HostName=iot-practica.azure-devices.net;DeviceId=dispositivo1;SharedAccessKey=wwgjtEUnTme346smwktXc4gp5GQXC8FyqSx0i7AZ8RM=';

const client = Client.fromConnectionString(connectionString, Mqtt);

setInterval(() => {
  const data = JSON.stringify({
    temperatura: 20 + Math.random() * 10,
    humedad: 40 + Math.random() * 20
  });

  const message = new Message(data);
  client.sendEvent(message);
  console.log("Mensaje enviado:", data);
}, 5000);