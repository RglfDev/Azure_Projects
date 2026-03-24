using CosmosDbFamily;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using System.Net;

class Program
{
    // The Azure Cosmos DB endpoint for running this sample.
    private static readonly string EndpointUri = "https://cosmosfamilydemolobo.documents.azure.com:443/";
    // The primary key for the Azure Cosmos account.
    private static readonly string PrimaryKey = "juSC6UFgRbBVpedFIbkATITCsBJ2yUdycATrqujDiR4TXbt87c2mIxOejdjSdA9YFA5R4Kkas3ywACDbiOrD4Q==";

    // The Cosmos client instance
    private static CosmosClient? cosmosClient;

    // The database we will create
    private static Database? database;

    // The container we will create.
    private static Container? container;

    // The name of the database and container we will create
    private static string databaseId = "FamiliesDB";
    private static string containerId = "Families";


    private static string storedProcedureFile = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName,
            "insertFamily.js");


    public static async Task Main(string[] args)
    {

        try
        {
            Console.WriteLine("Begin Cosmosdb SQL Api Demo...\n");

            // Create a new instance of the Cosmos Client
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

            // Create a new database
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", database.Id);

            // Create a new container with partition key
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/lastName");
            Console.WriteLine("Created Container: {0}\n", container.Id);

            // Registrar procedimiento almacenado
            await CreateStoredProcedureFromFileAsync(storedProcedureFile);

            //Insert
            await AddItemsToContainerAsync();

            //Query
            await QueryItemsAsync();

            //Update
            await ReplaceFamilyItemAsync();

            // Ejecutar el procedimiento almacenado
            await ExecuteStoredProcedureAsync();

            //Delete
            await DeleteFamilyItemAsync();

            //Delete Database
            await DeleteDatabaseAndCleanupAsync();
        }
        catch (CosmosException de)
        {
            Exception baseException = de.GetBaseException();
            Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e);
        }
        finally
        {
            Console.WriteLine("End of demo, press any key to exit.");
            Console.ReadKey();
        }
    }
    /// <summary>
    /// Add Family items to the container
    /// </summary>
    private static async Task AddItemsToContainerAsync()
    {
        // Create a family object for the Andersen family
        Family andersenFamily = new Family
        {
            Id = "Andersen.1",
            LastName = "Andersen",
            Parents = new Parent[]
            {
                new Parent { FirstName = "Thomas" },
                new Parent { FirstName = "Mary Kay" }
            },
            Children = new Child[]
            {
                new Child
                {
                    FirstName = "Henriette Thaulow",
                    Gender = "female",
                    Grade = 5,
                    Pets = new Pet[]
                    {
                        new Pet { GivenName = "Fluffy" }
                    }
                }
            },
            Address = new Address { State = "WA", County = "King", City = "Seattle" },
            IsRegistered = false
        };

        try
        {
            // Read the item to see if it exists.  
            ItemResponse<Family> andersenFamilyResponse = await container.ReadItemAsync<Family>(andersenFamily.Id, new PartitionKey(andersenFamily.LastName));
            Console.WriteLine("Item in database with id: {0} already exists\n", andersenFamilyResponse.Resource.Id);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<Family> andersenFamilyResponse = await container.CreateItemAsync<Family>(andersenFamily, new PartitionKey(andersenFamily.LastName));

            // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", andersenFamilyResponse.Resource.Id, andersenFamilyResponse.RequestCharge);
        }
        // Create a family object for the Wakefield family
        Family wakefieldFamily = new Family
        {
            Id = "Wakefield.7",
            LastName = "Wakefield",
            Parents = new Parent[]
            {
                new Parent { FamilyName = "Wakefield", FirstName = "Robin" },
                new Parent { FamilyName = "Miller", FirstName = "Ben" }
            },
            Children = new Child[]
            {
                new Child
                {
                    FamilyName = "Merriam",
                    FirstName = "Jesse",
                    Gender = "female",
                    Grade = 8,
                    Pets = new Pet[]
                    {
                        new Pet { GivenName = "Goofy" },
                        new Pet { GivenName = "Shadow" }
                    }
                },
                new Child
                {
                    FamilyName = "Miller",
                    FirstName = "Lisa",
                    Gender = "female",
                    Grade = 1
                }
            },
            Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
            IsRegistered = true
        };

        try
        {
            // Read the item to see if it exists
            ItemResponse<Family> wakefieldFamilyResponse = await container.ReadItemAsync<Family>(wakefieldFamily.Id, new PartitionKey(wakefieldFamily.LastName));
            Console.WriteLine("Item in database with id: {0} already exists\n", wakefieldFamilyResponse.Resource.Id);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Create an item in the container representing the Wakefield family. Note we provide the value of the partition key for this item, which is "Wakefield"
            ItemResponse<Family> wakefieldFamilyResponse = await container.CreateItemAsync<Family>(wakefieldFamily, new PartitionKey(wakefieldFamily.LastName));

            // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", wakefieldFamilyResponse.Resource.Id, wakefieldFamilyResponse.RequestCharge);
        }
    }
    /// <summary>
    /// Run a query (using Azure Cosmos DB SQL syntax) against the container
    /// </summary>
    private static async Task QueryItemsAsync()
    {
        var sqlQueryText = "SELECT * FROM c WHERE c.LastName = 'Andersen'";

        Console.WriteLine("Running query: {0}\n", sqlQueryText);

        QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
        FeedIterator<Family> queryResultSetIterator = container.GetItemQueryIterator<Family>(queryDefinition);

        List<Family> families = new List<Family>();

        while (queryResultSetIterator.HasMoreResults)
        {
            FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
            foreach (Family family in currentResultSet)
            {
                families.Add(family);
                Console.WriteLine("\tRead {0}\n", family);
            }
        }
    }
    /// <summary>
    /// Replace an item in the container
    /// </summary>
    private static async Task ReplaceFamilyItemAsync()
    {
        ItemResponse<Family> wakefieldFamilyResponse = await container.ReadItemAsync<Family>("Wakefield.7", new PartitionKey("Wakefield"));
        var itemBody = wakefieldFamilyResponse.Resource;

        // update registration status from false to true
        itemBody.IsRegistered = true;
        // update grade of child
        itemBody.Children[0].Grade = 6;

        // replace the item with the updated content
        wakefieldFamilyResponse = await container.ReplaceItemAsync<Family>(itemBody, itemBody.Id, new PartitionKey(itemBody.LastName));
        Console.WriteLine("Updated Family [{0},{1}].\n \tBody is now: {2}\n", itemBody.LastName, itemBody.Id, wakefieldFamilyResponse.Resource);
    }
    /// <summary>
    /// Delete an item in the container
    /// </summary>
    private static async Task DeleteFamilyItemAsync()
    {
        var partitionKeyValue = "Wakefield";
        var familyId = "Wakefield.7";

        // Delete an item. Note we must provide the partition key value and id of the item to delete
        ItemResponse<Family> wakefieldFamilyResponse =
            await container.DeleteItemAsync<Family>(familyId, new PartitionKey(partitionKeyValue));
        Console.WriteLine("Deleted Family [{0},{1}]\n", partitionKeyValue, familyId);
    }
    /// <summary>
    /// Delete the database and dispose of the Cosmos Client instance
    /// </summary>
    private static async Task DeleteDatabaseAndCleanupAsync()
    {
        DatabaseResponse databaseResourceResponse = await database.DeleteAsync();
        // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

        Console.WriteLine("Deleted Database: {0}\n", databaseId);

        //Dispose of CosmosClient
        cosmosClient.Dispose();
    }

    /// <summary>
    /// Crear el procedimiento almacenado desde un archivo
    /// </summary>
    private static async Task CreateStoredProcedureFromFileAsync(string scriptPath)
    {
        if (!File.Exists(scriptPath))
        {
            Console.WriteLine($"Error: El archivo {scriptPath} no existe.");
            return;
        }

        string scriptBody = await File.ReadAllTextAsync(scriptPath);

        var storedProcedureDefinition = new StoredProcedureProperties
        {
            Id = "insertFamily",
            Body = scriptBody
        };

        try
        {
            await container.Scripts.ReplaceStoredProcedureAsync(storedProcedureDefinition);
            Console.WriteLine("Procedimiento almacenado 'insertFamily' registrado correctamente.\n");
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            await container.Scripts.CreateStoredProcedureAsync(storedProcedureDefinition);
            Console.WriteLine("Procedimiento almacenado 'insertFamily' creado correctamente.\n");
        }

    }

    private static async Task ExecuteStoredProcedureAsync()
    {
        string storedProcedureId = "insertFamily"; // Nombre del SP en Azure
        string partitionKey = "Andersen"; // Debe coincidir con la clave de partición de los datos

        // Parámetros del SP (deben coincidir con lo que espera en el JS)
        var parameters = new dynamic[]
        {
        new
        {
            id = "Andersen.2",
            LastName = "Andersen",
            Parents = new[] { new { FirstName = "John" }, new { FirstName = "Jane" } },
            Children = new[] { new { FirstName = "Anna", Gender = "female", Grade = 3 } },
            Address = new { State = "WA", County = "King", City = "Seattle" },
            IsRegistered = true
        }
        };

        try
        {
            var response = await container.Scripts.ExecuteStoredProcedureAsync<StoredProcedureResponse>(
                storedProcedureId,
                new PartitionKey(partitionKey),
                parameters
            );

            Console.WriteLine("Stored Procedure executed successfully.");
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Error executing stored procedure: {ex.Message}");
            Console.WriteLine($"StatusCode: {ex.StatusCode}, Message: {ex.Message}");
        }

    }
}

public class StoredProcedureResponse
{
    public string? Status { get; set; }
    public string? Message { get; set; }
}