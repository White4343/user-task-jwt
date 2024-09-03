using Microsoft.EntityFrameworkCore;
using UserTaskJWT.Web.Api.Data.EntityConfigurations;
using UserTaskJWT.Web.Api.Users;
using Task = UserTaskJWT.Web.Api.Tasks.Task;

namespace UserTaskJWT.Web.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User>? Users { get; set; }
        
        public DbSet<Task>? Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TaskEntityConfiguration());
        }
    }
}