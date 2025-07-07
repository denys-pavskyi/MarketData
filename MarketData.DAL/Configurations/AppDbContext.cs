using MarketData.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketData.DAL.Configurations;

public class AppDbContext : DbContext
{
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetMapping> AssetMappings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


    }

}