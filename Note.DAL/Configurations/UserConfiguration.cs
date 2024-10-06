using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity;

namespace Note.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(user => user.Id).ValueGeneratedOnAdd();
            builder.Property(user => user.Login).IsRequired().HasMaxLength(120);
            builder.Property(user => user.Password).IsRequired();
            builder.HasMany(user => user.Reports).WithOne(report => report.User)
            .HasForeignKey(report => report.UserId)
            .HasPrincipalKey(user => user.Id);

            builder.HasData(new List<User>()
            {
                new User()
                {
                    Id = 1,
                    Login = "user1",
                    Password = "veververervrevverervv",
                    CreatedAt = DateTime.UtcNow
                },
                new User()
                {
                    Id = 2,
                    Login = "user2",
                    Password = "tesrjigbejrfhibugehjrfiu",
                    CreatedAt = DateTime.UtcNow
                }
            });

        }
    }
}
