using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductosWebAPI.Models;

namespace ProductosWebAPI.Context
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class Categories_DBController : ControllerBase
    {
        private readonly AppDbContext _context;
        public Categories_DBController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories_DB>>> GetCategories()
        {
            List<Categories_DB> categories = new List<Categories_DB>();
            categories = await _context.Categories.ToListAsync();
            return await _context.Categories.ToListAsync();
        }
        // GET: api/Categories_DB/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categories_DB>> GetCategories_DB(int id)
        {
            var category_DB = await _context.Categories.FindAsync(id);

            if (category_DB == null)
            {
                return NotFound();
            }

            return category_DB;
        }
    }
}
