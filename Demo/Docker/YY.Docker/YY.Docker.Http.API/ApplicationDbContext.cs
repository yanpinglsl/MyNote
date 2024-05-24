using Microsoft.EntityFrameworkCore;
using YY.Docker.Http.API.Models;

namespace YY.Docker.Http.API
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Person> Person { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}
