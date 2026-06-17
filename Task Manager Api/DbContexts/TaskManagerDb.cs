using Microsoft.EntityFrameworkCore;
using Task_Manager_Api.Models;

namespace Task_Manager_Api.DbContexts;

public class TaskManagerDb : DbContext
{
    public TaskManagerDb (DbContextOptions<TaskManagerDb>options) : base(options)
    {

    }

    public DbSet<TodoItem> TodoItems { get; set; }

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
            .Property(t => t.Description)
            .HasMaxLength(500);
             
    }
}
