using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity.ChatEntity;

namespace Note.DAL.Configurations.ChatConfigurations;

public class ChatConfigurations : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.User1).IsRequired();
        builder.Property(x => x.User2).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
