namespace MakeAPI_Cosmos.Models
{
    public interface ICarCosmosService
    {
        Task<List<Car>> GetCars(string sqlCosmosQuery);
        Task<Car> Add(Car newCar);
        Task<Car> Update(Car carToUpdate);
        Task Delete(string id,string make);
    }
}
