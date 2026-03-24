using MakeAPI_Cosmos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MakeAPI_Cosmos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarCosmosService _carCosmosService;
        public CarController(ICarCosmosService carCosmosService)
        {
            _carCosmosService = carCosmosService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var sqlCosmosQuery = "Select * from c";
            var result = await _carCosmosService.GetCars(sqlCosmosQuery);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Car newCar)
        {
            newCar.Id = Guid.NewGuid().ToString();
            await _carCosmosService.Add(newCar);
            return CreatedAtAction(nameof(Get), new {id=newCar.Id},newCar);
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Car carToUpdate)
        {
            await _carCosmosService.Update(carToUpdate);
            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string id,string make)
        {
            await _carCosmosService.Delete(id,make);
            return NoContent();
        }
    }
}
