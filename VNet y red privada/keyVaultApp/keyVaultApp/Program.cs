using Azure;

using Azure.Identity;

using Azure.Security.KeyVault.Certificates;




class Program

{

    static async Task Main(string[] args)

    {

        string keyVaultName = "mitajamarsecretslobo "; // Ajusta tu Key Vault 

        string certificateName = "miCertificado1";

        string kvUrl = $"https://{keyVaultName}.vault.azure.net/";



        var credential = new DefaultAzureCredential();

        var certificateClient = new CertificateClient(new Uri(kvUrl), credential);



        KeyVaultCertificate certificate = null;



        try

        {

            // 1️. Intentar obtener el certificado existente 

            certificate = await certificateClient.GetCertificateAsync(certificateName);

            Console.WriteLine($"Certificado encontrado: {certificate.Name}, ID: {certificate.Id}");

        }

        catch (RequestFailedException ex) when (ex.Status == 404)

        {

            // El certificado no existe → crear uno nuevo self-signed 

            Console.WriteLine($"El certificado '{certificateName}' no existe. Creando un nuevo certificado autofirmado...");



            var certPolicy = new CertificatePolicy("Self", "CN=mi-certificado1.local")

            {

                KeyType = "RSA",

                KeySize = 2048,

                Exportable = true,

                ReuseKey = true

            };



            try

            {

                CertificateOperation certOperation = await certificateClient.StartCreateCertificateAsync(certificateName, certPolicy);

                certificate = await certOperation.WaitForCompletionAsync();

                Console.WriteLine($"Certificado creado: {certificate.Name}, ID: {certificate.Id}");

            }

            catch (RequestFailedException createEx)

            {

                Console.WriteLine("Error al crear el certificado:");

                Console.WriteLine($"Status: {createEx.Status}");

                Console.WriteLine($"Message: {createEx.Message}");

            }

        }

        catch (RequestFailedException ex)

        {

            // Otro error (permisos RBAC, etc.) 

            Console.WriteLine("Error al acceder al Key Vault:");

            Console.WriteLine($"Status: {ex.Status}");

            Console.WriteLine($"Message: {ex.Message}");

        }



        // 2️. Mostrar información del certificado 

        if (certificate != null)

        {

            Console.WriteLine("\nInformación del certificado:");

            Console.WriteLine($"Nombre: {certificate.Name}");

            Console.WriteLine($"ID: {certificate.Id}");

            Console.WriteLine($"Versión: {certificate.Properties.Version}");

            Console.WriteLine($"Creado: {certificate.Properties.CreatedOn}");

            Console.WriteLine($"Expira: {certificate.Properties.ExpiresOn}");

        }



        // 3️. Eliminar el certificado (opcional) 

        Console.WriteLine("\n¿Deseas eliminar el certificado? (s/n)");

        string input = Console.ReadLine();

        if (input?.Trim().ToLower() == "s")

        {

            try

            {

                DeleteCertificateOperation deleteOperation = await certificateClient.StartDeleteCertificateAsync(certificateName);

                await deleteOperation.WaitForCompletionAsync();

                Console.WriteLine($"Certificado eliminado: {certificateName}");

            }

            catch (RequestFailedException deleteEx)

            {

                Console.WriteLine("Error al eliminar el certificado:");

                Console.WriteLine($"Status: {deleteEx.Status}");

                Console.WriteLine($"Message: {deleteEx.Message}");

            }

        }



        // Mantener la consola abierta 

        Console.WriteLine("\nPresiona cualquier tecla para salir...");

        Console.ReadKey();

    }

}