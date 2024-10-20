using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public virtual DbSet<Image> Images { get; set; }
    public virtual DbSet<Answer> Answers { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<Film> Films { get; set; }

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