//HostName=iot-tajamarpruebaslobo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=yG34NfdY7lMgtB+WO5sYoKNcf0YuwB/8yAIoTKJCOQg=

using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

public class Program
{
    static RegistryManager? registryManager;

    static string IoTHubConnectionString = "HostName=iot-tajamarpruebaslobo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=yG34NfdY7lMgtB+WO5sYoKNcf0YuwB/8yAIoTKJCOQg=";
    static string targetDevice = "Device01";

    static bool running = true;

    public static async Task SimulateTelemetry()
    {
        Random rnd = new Random();

        while (running)
        {
            Twin twin = await registryManager!.GetTwinAsync(targetDevice);

            TwinCollection reportedProperties = new TwinCollection();
            TwinCollection deviceData = new TwinCollection();

            deviceData["temperature"] = Math.Round(20 + rnd.NextDouble() * 15, 2);
            deviceData["pressure"] = Math.Round(950 + rnd.NextDouble() * 100, 2);
            deviceData["humidity"] = Math.Round(30 + rnd.NextDouble() * 50, 2);
            deviceData["batteryLevel"] = rnd.Next(20, 100);
            deviceData["status"] = "Running";
            deviceData["timestamp"] = DateTime.Now;

            reportedProperties["deviceTelemetry"] = deviceData;

            await registryManager.UpdateTwinAsync(
                targetDevice,
                reportedProperties.ToJson(),
                twin.ETag);

            Console.WriteLine($"Datos enviados → Temperatura: {deviceData["temperature"]} °C | Humedad: {deviceData["humidity"]}% | Nivel de batería: {deviceData["batteryLevel"]}%");
            await Task.Delay(5000);
        }
    }

    static void Main()
    {
        registryManager = RegistryManager.CreateFromConnectionString(IoTHubConnectionString);

        Console.WriteLine("Simulación de telemetría iniciada (cada 5 segundos)");
        Console.WriteLine("Pulsa ENTER para detener...\n");

        // Ejecutar simulación en segundo plano
        var task = SimulateTelemetry();

        Console.ReadLine();
        running = false;

        task.Wait();

        Console.WriteLine("Simulación detenida");
    }
}
