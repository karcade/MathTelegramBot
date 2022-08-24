using MathBot.Model.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathBot.Model
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Number> Numbers { get; set; } = null!;
        public DbSet<Exercise> Exercises { get; set; } = null!;
        public DbSet<Test> Tests { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Number>()
            .HasOne(u => u.Exercise)
            .WithMany(c => c.Numbers).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exercise>()
            .HasOne(u => u.Test)
            .WithMany(p => p.Exercises).OnDelete(DeleteBehavior.Cascade);

            /*modelBuilder
            .Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserKey);*/

            modelBuilder.Entity<Test>()
           .HasOne(u => u.User)
           .WithMany(c => c.Tests).OnDelete(DeleteBehavior.Cascade);


            /*modelBuilder.Entity<User>()
            .HasMany(u => u.Tests)
            .WithOne(c => c.User)
            .OnDelete(DeleteBehavior.Cascade);*/
        }
    }
}
