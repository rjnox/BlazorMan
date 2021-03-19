using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorMan.Data
{
    public class BlazorManContext : IdentityDbContext<IdentityUser>
    {
        public BlazorManContext(DbContextOptions<BlazorManContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        // The following configures EF to create a Sqlite database file as `C:\blazorman.db`.
        // For Mac or Linux, change this to `/tmp/blazorman.db` or any other absolute path.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=C:\blazorman.db");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<Score> Scores { get; set; }
    }
}
