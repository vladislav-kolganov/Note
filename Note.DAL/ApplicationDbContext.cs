using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Note.Domain.Entity.ChatEntity;
using Note.Domain.Helpers;
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

        ConfigureEncryption(modelBuilder);
    }

    private void ConfigureEncryption(ModelBuilder modelBuilder)
    {
        var keyBase64 = _configuration["EncryptionSettings:Key"]
            ?? throw new InvalidOperationException("EncryptionSettings:Key не задан в конфигурации.");
        var key = Convert.FromBase64String(keyBase64);

        var textConverter = new ValueConverter<string, string>(
            v => AesEncryptionHelper.Encrypt(v, key),
            v => AesEncryptionHelper.Decrypt(v, key));

        var bytesConverter = new ValueConverter<byte[], byte[]>(
            v => AesEncryptionHelper.EncryptBytes(v, key),
            v => AesEncryptionHelper.DecryptBytes(v, key));

        modelBuilder.Entity<Message>(e =>
            e.Property(m => m.TextMessage).HasConversion(textConverter));

        modelBuilder.Entity<MessagePhoto>(e =>
            e.Property(p => p.Content).HasConversion(bytesConverter));
    }
}
