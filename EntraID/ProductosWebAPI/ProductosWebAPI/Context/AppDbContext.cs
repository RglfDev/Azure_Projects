using Microsoft.EntityFrameworkCore;
using ProductosWebAPI.Models;

namespace ProductosWebAPI.Context
{
    public class AppDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Products_DB> Products { get; set; }
        public DbSet<Categories_DB> Categories { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelbuilder)
        //{
        //    modelbuilder.Seed();
        //    base.OnModelCreating(modelbuilder);
        //}

    }
}
