using Microsoft.EntityFrameworkCore;
using ServerProject.Models;
namespace ServerProject.Configurations
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerDemographic>().HasNoKey();

          
        }
        public DbSet<Customer> Customers { get; set; }
    }

}
