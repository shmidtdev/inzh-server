using IngServer.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace IngServer.DataBase;

public sealed class ApplicationContext : DbContext
{
    public DbSet<Category> Categories { get; init; } = null!;
    public DbSet<Product> Products { get; init; } = null!;
    public DbSet<Image> Images { get; init; } = null!;
    public DbSet<Characteristic> Characteristics { get; init; } = null!;
    public DbSet<Order> Orders { get; init; } = null!;
    public DbSet<WishList> WishLists { get; init; } = null!;
    public DbSet<User> Users { get; init; } = null!;
    public DbSet<Subscriber> Subscribers { get; init; } = null!;
    public DbSet<Article> Articles { get; init; } = null!;
    public DbSet<ProductMovement> ProductMovements { get; init; } = null!;
    public DbSet<CategoryInfo> CategoryInfos { get; init; } = null!;

    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseNpgsql("Host=localhost;Port=2345;Database=postgres;Username=admin;Password=1337");
        optionsBuilder.UseNpgsql("Host=194.67.105.245;Port=5432;Database=postgres;Username=postgres;Password=1337");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Characteristic>().HasIndex(c => new { c.NameEng, c.ValueEng });
        modelBuilder.Entity<Category>().HasIndex(c => new { c.NameEng, c.Id }); 
        modelBuilder.Entity<Product>()
            .HasGeneratedTsVectorColumn(p => p.SearchVector, "russian", p => new { p.Title })
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");

        modelBuilder.Entity<CategoryInfo>().HasMany(c => c.BottomChildren);

        modelBuilder.Entity<Product>().HasMany(p => p.Characteristics);
    }
}