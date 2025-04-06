using Microsoft.EntityFrameworkCore;
using DiscordBotClasses.Models;

namespace DiscordDBConnection
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Test> Tests { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ExampleTest> ExampleTests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=localdb.db");  

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Test)   
                .WithMany(t => t.Documents)  
                .HasForeignKey(d => d.TestId);  

            modelBuilder.Entity<ExampleTest>()
                .HasOne(e => e.Test)   
                .WithMany(t => t.ExampleTests)  
                .HasForeignKey(e => e.TestId);  
        }
    }
}
