using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618
namespace RutrackerDumpParser;

public class RutrackerContext : DbContext
{
    public static RutrackerContext CreateContext()
    {
        var dbContext = new RutrackerContext();
        dbContext.Database.EnsureCreated();
        return dbContext;
    }

    public DbSet<TorrentRoot> Torrents { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite("Data Source=rutracker.db");
    }
}
