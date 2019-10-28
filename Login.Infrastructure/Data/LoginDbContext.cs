using Microsoft.EntityFrameworkCore;

namespace Login.Infrastructure.Data
{
    public class LoginDbContext : DbContext
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }

        public DbSet<User> User { get; set; }
        public DbSet<ActiveDirectoryUser> ActiveDirectoryUser { get; set; }
    }
}