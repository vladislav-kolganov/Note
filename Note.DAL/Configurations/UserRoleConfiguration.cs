using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity;

namespace Note.DAL.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.Property(ur => ur.RoleId).IsRequired();
            builder.Property(ur => ur.UserId).IsRequired();

            builder.HasData(
                new List<UserRole>()
                {
                    new UserRole ()
                   {
                        RoleId = 2,
                        UserId = 1,
                   }
                }
            );
        }
    }
}
