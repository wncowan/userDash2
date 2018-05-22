using Microsoft.EntityFrameworkCore;

namespace userDash2.Models
{
    public class DashboardContext : DbContext
    {
        // INCLUDE ALL MODELS AS DBSETS: ie. DbSet<User> Users { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DashboardContext(DbContextOptions<DashboardContext> options) : base(options)
        { }
    }
}
