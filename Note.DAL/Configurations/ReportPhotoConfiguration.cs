using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity;
using Note.Domain.Entity.ChatEntity;

namespace Note.DAL.Configurations;

/// <summary>
/// Конфигурация модели ReportPhoto в БД.
/// </summary>
public class ReportPhotoConfiguration : IEntityTypeConfiguration<ReportPhoto>
{
    public void Configure(EntityTypeBuilder<ReportPhoto> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Content)
            .IsRequired()
            .HasColumnType("bytea");
    }
}