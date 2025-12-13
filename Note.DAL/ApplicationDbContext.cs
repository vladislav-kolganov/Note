using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Note.Domain.Settings.DbSettings;

namespace Note.DAL;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
        Database.EnsureCreated();
    }

    public ApplicationDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = _configuration.GetSection(nameof(PostgresSettings)).Get<PostgresSettings>() ??
            throw new ArgumentNullException(nameof(PostgresSettings) + " is null");

        optionsBuilder.UseNpgsql(config.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
