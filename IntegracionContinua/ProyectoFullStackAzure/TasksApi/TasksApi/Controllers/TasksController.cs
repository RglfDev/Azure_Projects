using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TasksApi.Models;
using TasksApi.Services;

namespace TasksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly CosmosService? _service;

        public TasksController(CosmosService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service!.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post(TaskItem item)
        {
            await _service!.AddAsync(item);
            return Ok(item);
        }

    }
}
