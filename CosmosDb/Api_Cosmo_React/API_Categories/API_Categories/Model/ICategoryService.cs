namespace API_Categories.Model
{

    public interface ICategoryCosmosService
    {
        Task<List<Category>> GetCategories(string sqlCosmosQuery);
        Task<Category> Add(Category newCategory);
        Task<Category> Update(Category categoryToUpdate);
        Task Delete(string id, string categoryName); // partition key = CategoryName
    }
}

