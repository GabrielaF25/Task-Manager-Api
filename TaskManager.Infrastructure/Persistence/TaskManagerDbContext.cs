using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.DbContexts;

public class TaskManagerDbContext : DbContext
{
    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
    {

    }

    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>()
            .ToTable("Todos");
            
        modelBuilder.Entity<TodoItem>()
            .Property(t => t.IsCompleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<TodoItem>()
            .Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<TodoItem>()
           .Property(t => t.CreatedAt)
           .IsRequired();

        modelBuilder.Entity<TodoItem>()
            .Property(t => t.Description)
            .HasMaxLength(500);

        modelBuilder.Entity<TodoItem>()
            .HasOne(t => t.Project)  
            .WithMany(p => p.TodoItems)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Project>()
            .ToTable("Projects");

        modelBuilder.Entity<Project>()
            .Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Project>()
            .Property(p => p.Description)
            .HasMaxLength(500);


    }
}
