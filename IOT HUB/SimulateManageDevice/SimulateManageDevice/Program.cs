
//HostName=iot-tajamarpruebaslobo.azure-devices.net;DeviceId=Device01;SharedAccessKey=Nplj2b9Frv/ni6+u5jPVduM8AtJK3OgN4rJ4qMqAK/4=


using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Text;
 
namespace SimulateManagedDevice
{
    public class Program
    {
        static string DeviceConnectionString = "HostName=iot-tajamarpruebaslobo.azure-devices.net;DeviceId=Device01;SharedAccessKey=Nplj2b9Frv/ni6+u5jPVduM8AtJK3OgN4rJ4qMqAK/4=";
        static DeviceClient Client = null!;

        static Task<MethodResponse> onReboot(MethodRequest methodRequest, object userContext)
        {
            try
            {
                Console.WriteLine("Rebooting simulated device...");

                TwinCollection reportedProperties = new TwinCollection();
                TwinCollection deviceData = new TwinCollection();

                Random rnd = new Random();

                // Simulación de sensores
                deviceData["temperature"] = Math.Round(20 + rnd.NextDouble() * 15, 2); // 20-35 ºC
                deviceData["pressure"] = Math.Round(950 + rnd.NextDouble() * 100, 2); // 950-1050 hPa
                deviceData["humidity"] = Math.Round(30 + rnd.NextDouble() * 50, 2);   // 30-80 %
                deviceData["batteryLevel"] = rnd.Next(20, 100); // %
                deviceData["status"] = "Running";
                deviceData["lastReboot"] = DateTime.Now;


                reportedProperties["deviceTelemetry"] = deviceData;

                Client.UpdateReportedPropertiesAsync(reportedProperties).Wait();

                Console.WriteLine("Telemetry updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }

            string result = @"{""result"":""Reboot completed and telemetry updated.""}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        static void Main()
        {
            try
            {
                Console.WriteLine("Connecting to IoT Hub...");

                Client = DeviceClient.CreateFromConnectionString(
                    DeviceConnectionString,
                    TransportType.Mqtt);

                // Registrar método reboot
                Client.SetMethodHandlerAsync("reboot", onReboot, null).Wait();

                Console.WriteLine("Waiting for reboot method...\nPress ENTER to exit.");
                Console.ReadLine();

                Console.WriteLine("Exiting...");

                Client.SetMethodHandlerAsync("reboot", null, null).Wait();
                Client.CloseAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }
    }
}