using Microsoft.Azure.Cosmos;

namespace MakeAPI_Cosmos.Models
{
    public class CarCosmosService: ICarCosmosService
    {
        private readonly Container _container;
        public CarCosmosService(CosmosClient cosmosDbClient, string databaseName, string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName,containerName);
        }

        public async Task<List<Car>> GetCars(string sqlCosmosQuery)
        {
            var query = _container.GetItemQueryIterator<Car>(new QueryDefinition(sqlCosmosQuery));
            List<Car> result = new List<Car>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                result.AddRange(response);
            }
            return result;
        }
        public async Task<Car> Add(Car newCar)
        {
            var item = await _container.CreateItemAsync(newCar,new PartitionKey(newCar.Make));
            return item;
        }

        public async Task<Car> Update(Car carToUpdate)
        {
            var item = await _container.UpsertItemAsync<Car>(carToUpdate, new PartitionKey(carToUpdate.Make));
            return item;
        }

        public async Task Delete(string id, string make)
        {
            await _container.DeleteItemAsync<Car>(id, new PartitionKey(make));
        }
    }
}
