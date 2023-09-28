using Microsoft.EntityFrameworkCore;
using RedditClone.Models; 
namespace RedditClone.Data
{
    public class RedditCloneContext : DbContext
    {
        public RedditCloneContext(DbContextOptions<RedditCloneContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Restricting delete behavior to prevent multiple cascade paths

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany() // Replace with the navigation property in Post if exists
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction); 

             modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne() 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
