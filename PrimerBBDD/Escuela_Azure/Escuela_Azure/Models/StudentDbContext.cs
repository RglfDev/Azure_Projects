using Microsoft.EntityFrameworkCore;

namespace Escuela_Azure.Models
{
    public class StudentDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Student> Students { get; set; }

        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        { }

    }
}
