using Bookmarks.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookmarks.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Bookmark> Bookmarks { get; set; }
    }

}