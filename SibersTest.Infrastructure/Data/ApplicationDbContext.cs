using Microsoft.EntityFrameworkCore;
using SibersTest.Core.Entities;

namespace SibersTest.Infrastructure.Data
{
    /// <summary>
    /// Database context for the application, using Entity Framework Core. 
    /// It defines the DbSet properties for Projects and Employees, and configures the relationships between them in the OnModelCreating method.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Employee> Employees { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-many relationship between Project and Employee (Manager)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Manager)
                .WithMany(e => e.ManagedProjects)
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict); 
                // Save the manager when deleting a project, and prevent deleting a manager if they have projects assigned.

            // Many-to-many relationship between Projects and Employees
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Employees)
                .WithMany(e => e.Projects)
                .UsingEntity(j => j.ToTable("ProjectEmployees")); 
                // Create a join table for the many-to-many relationship.


            base.OnModelCreating(modelBuilder);

        }

    }
}
