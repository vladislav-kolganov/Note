using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure( EntityTypeBuilder <User> builder)
        {
            builder.Property(user => user.Id).ValueGeneratedOnAdd(); // генерация id при добавлении записи
            builder.Property(user => user.Login).IsRequired().HasMaxLength(120); // ограничиваем длинну логина & not null
            builder.Property(user => user.Password).IsRequired();
            builder.HasMany(user => user.Reports).WithOne(report => report.User)
            .HasForeignKey(report => report.UserId)
            .HasPrincipalKey(user => user.Id);
           
        }
    }
}
