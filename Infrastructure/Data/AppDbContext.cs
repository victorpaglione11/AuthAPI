using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }

    public DbSet<UserRefreshToken> RefreshTokens { get; set; }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<IAuditable>()
            .Where(e => e.State == EntityState.Added);

        foreach (var entityEntry in entries)
        {
            entityEntry.Entity.CreatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

}