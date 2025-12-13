using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity.ChatEntity;

namespace Note.DAL.Configurations.ChatConfigurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.TextMessage).HasMaxLength(10000);
        builder.Property(x => x.ProducerMessageId).IsRequired();
        builder.Property(x => x.ConsumerMessageId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasMany(message => message.Photos).
                WithOne(messagePhoto => messagePhoto.Message).
                HasForeignKey(messagePhoto => messagePhoto.MessageId).
                OnDelete(DeleteBehavior.Cascade);
    }
}
