using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Do.API.Entities
{
    public class DoContext : DbContext
    {
        public DoContext(DbContextOptions<DoContext> options)
           : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        // public DbSet<BlogPostTask> BlogPostTasks { get; set; }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPostTask>()
                .HasKey(bpt => new { bpt.BlogPostId, bpt.TaskId });
        }*/
    }
}
