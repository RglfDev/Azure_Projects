using API_Categories.Model;
using Microsoft.Azure.Cosmos;

namespace API_Categories.Model
{
   
    public class CategoryCosmosService : ICategoryCosmosService
    {
        private readonly Container _container;
        public CategoryCosmosService(CosmosClient cosmosClient,
            string databaseName, string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<List<Category>> GetCategories(string sqlCosmosQuery)
        {
            var query = _container.GetItemQueryIterator<Category>(new QueryDefinition(sqlCosmosQuery));
            List<Category> result = new List<Category>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                result.AddRange(response);
            }
            return result;
        }

        public async Task<Category> Add(Category newCategory)
        {
            var item = await _container.CreateItemAsync(newCategory, new PartitionKey(newCategory.CategoryID));
            return item.Resource;
        }

        public async Task<Category> Update(Category categoryToUpdate)
        {
            var item = await _container.UpsertItemAsync(categoryToUpdate, new PartitionKey(categoryToUpdate.CategoryID));
            return item.Resource;
        }

        public async Task Delete(string id, string categoryName)
        {
            await _container.DeleteItemAsync<Category>(id, new PartitionKey(id)); // partition key = id
        }
    }
}
