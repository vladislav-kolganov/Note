using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity.ChatEntity;

namespace Note.DAL.Configurations.ChatConfigurations;

/// <summary>
/// Настройки конфигурации модели фото в БД.
/// </summary>
public class MessagePhotoConfiguration : IEntityTypeConfiguration<MessagePhoto>
{
    public void Configure(EntityTypeBuilder<MessagePhoto> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Content)
            .IsRequired()
            .HasColumnType("bytea");
    }
}