using Microsoft.EntityFrameworkCore;

namespace Login.Infrastructure.Data
{
    public class LoginDbContext : DbContext
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> options)
            : base(options) { }
        
        public DbSet<User> User { get; set; }
    }
}