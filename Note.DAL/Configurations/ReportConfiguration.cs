using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity;

namespace Note.DAL.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.Property(report => report.Id).ValueGeneratedOnAdd();
        builder.Property(report => report.Name).IsRequired().HasMaxLength(200);
        builder.Property(report => report.Description).IsRequired().HasMaxLength(2000);

        builder.HasMany(message => message.Photos).
        WithOne(reportPhoto => reportPhoto.Report).
        HasForeignKey(reportPhoto => reportPhoto.ReportId).
        OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.MapMarkers)
            .WithOne(x => x.Report)
            .HasForeignKey(x => x.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}