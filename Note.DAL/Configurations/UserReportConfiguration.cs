using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity;

namespace Note.DAL.Configurations;

public class UserReportConfiguration : IEntityTypeConfiguration<UserReport>
{
    public void Configure(EntityTypeBuilder<UserReport> builder)
    {
        builder.Property(ur => ur.ReportId).IsRequired();
        builder.Property(ur => ur.UserId).IsRequired();
        builder.Property(ur => ur.IsDeleteForThisUser)
               .HasColumnType("boolean")
               .HasDefaultValue(false)
               .IsRequired();
        builder.HasKey(ur => new { ur.UserId, ur.ReportId });
    }
}