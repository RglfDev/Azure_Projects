using Microsoft.Azure.Cosmos;
using TasksApi.Models;

namespace TasksApi.Services
{
    public class CosmosService
    {
        private readonly Container? _container;

        public CosmosService(IConfiguration config)
        {
            var client = new CosmosClient(
                config["CosmosDb:Account"],
                config["CosmosDb:Key"]
            );
            
            var database = client.GetDatabase("TaskDb");
            _container = database.GetContainer("Tasks");
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            var query = _container!.GetItemQueryIterator<TaskItem>();
            var results = new List<TaskItem>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task AddAsync(TaskItem item)
        {
            await _container!.CreateItemAsync(item, new PartitionKey(item.id));
        }
    }
}
