using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity.Map;

namespace Note.DAL.Configurations.MapConfigurations;

public class ReportMapMarkerConfiguration : IEntityTypeConfiguration<ReportMapMarker>
{
    public void Configure(EntityTypeBuilder<ReportMapMarker> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LocationName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.Comment)
            .HasMaxLength(2000);

        builder.Property(x => x.FireClass)
            .IsRequired();

        builder.Property(x => x.Latitude)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .IsRequired();

        builder.HasOne(x => x.Report)
            .WithMany(x => x.MapMarkers)
            .HasForeignKey(x => x.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
        .HasMany(x => x.Attachments)
        .WithOne(x => x.Marker)
        .HasForeignKey(x => x.MarkerId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ReportId);
    }
}