using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

/// <summary>
/// The application's database context that integrates with Identity for user management and extends 
/// <see cref="IdentityDbContext"/> to handle user authentication and authorization. 
/// Also manages entities for images, films, answers, questions, and test results.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    /// <summary>
    /// Gets or sets the <see cref="DbSet{Image}"/> for images in the application.
    /// </summary>
    public virtual DbSet<Image> Images { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{Answer}"/> for answers in the application.
    /// </summary>
    public virtual DbSet<Answer> Answers { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{Question}"/> for questions in the application.
    /// </summary>
    public virtual DbSet<Question> Questions { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{Film}"/> for films in the application.
    /// </summary>
    public virtual DbSet<Film> Films { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{TestResult}"/> for test results in the application.
    /// </summary>
    public virtual DbSet<TestResult> TestResults { get; set; }

    /// <summary>
    /// Configures the entity relationships and model properties for the database.
    /// This method is used to define custom behavior for the model.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure the entities.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         modelBuilder.Entity<Film>()
            .HasOne(f => f.Image)
            .WithOne(i => i.Film)
            .HasForeignKey<Image>(i => i.FilmId);

        modelBuilder.Entity<Image>()
            .HasOne(i => i.Film)
            .WithOne(f => f.Image)
            .HasForeignKey<Film>(f => f.ImageId); 

        base.OnModelCreating(modelBuilder);
    }
}