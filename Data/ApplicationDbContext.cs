using Microsoft.EntityFrameworkCore;
using Hackathon.Models;

namespace Hackathon.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
