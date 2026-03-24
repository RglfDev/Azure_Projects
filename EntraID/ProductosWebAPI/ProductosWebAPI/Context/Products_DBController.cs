using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class Products_DBController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Products_DBController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Products_DB
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products_DB>>> GetProducts()
        {
            return await _context.Products.Include(p=>p.Category).ToListAsync();
        }

        // GET: api/Products_DB/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Products_DB>> GetProducts_DB(int id)
        {
            //var products_DB = await _context.Products.FindAsync(id);
            var products_DB = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

            if (products_DB == null)
            {
                return NotFound();
            }

            return products_DB;
        }

        // PUT: api/Products_DB/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducts_DB(int id, Products_DB products_DB)
        {
            if (id != products_DB.Id)
            {
                return BadRequest();
            }

            _context.Entry(products_DB).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Products_DBExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products_DB
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Products_DB>> PostProducts_DB(Products_DB products_DB)
        {
            _context.Products.Add(products_DB);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducts_DB", new { id = products_DB.Id }, products_DB);
        }

        // DELETE: api/Products_DB/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducts_DB(int id)
        {
            var products_DB = await _context.Products.FindAsync(id);
            if (products_DB == null)
            {
                return NotFound();
            }

            _context.Products.Remove(products_DB);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Products_DBExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
