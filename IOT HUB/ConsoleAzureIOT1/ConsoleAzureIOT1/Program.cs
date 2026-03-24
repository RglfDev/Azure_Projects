
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
 
namespace ConsoleAzureIoT01
{
    class Program
    {
        static RegistryManager? _regitryManager;
        //Cadena de conexión al Centro de IoT
        private const string ConnectionString = "HostName=iot-tajamarpruebaslobo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=yG34NfdY7lMgtB+WO5sYoKNcf0YuwB/8yAIoTKJCOQg=";
        //Identificador del nuevo dispositivo
        private const string DeviceId = "Device01";

        static void Main()
        {
            //Registramos el Centro de IoT en la aplicación
            _regitryManager = RegistryManager.CreateFromConnectionString(ConnectionString);
            AddDeviceAsync().Wait();
        }

        private async static Task AddDeviceAsync()
        {
            //Creamos el dispositivo en caso de que no exista ya
            Device device;
            try
            {
                device = await _regitryManager!.AddDeviceAsync(new Device(DeviceId));
            }
            catch (DeviceAlreadyExistsException)
            {

                device = await _regitryManager!.GetDeviceAsync(DeviceId);
            }
            Console.WriteLine("Device Id: {0}", DeviceId);
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}




//HostName=iot-tajamarpruebaslobo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=yG34NfdY7lMgtB+WO5sYoKNcf0YuwB/8yAIoTKJCOQg=
