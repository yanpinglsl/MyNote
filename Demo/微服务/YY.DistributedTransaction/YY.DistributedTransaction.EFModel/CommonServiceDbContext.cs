using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.DistributedTransaction.EFModel
{
    public partial class CommonServiceDbContext : DbContext
    {
        public CommonServiceDbContext(DbContextOptions<CommonServiceDbContext> options) : base(options)
        {
            Console.WriteLine($"This is {nameof(CommonServiceDbContext)}  DbContextOptions");
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
