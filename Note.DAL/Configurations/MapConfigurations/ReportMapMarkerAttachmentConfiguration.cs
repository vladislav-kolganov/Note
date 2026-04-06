using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Note.Domain.Entity.Map;

namespace Note.DAL.Configurations.MapConfigurations;

public class ReportMapMarkerAttachmentConfiguration : IEntityTypeConfiguration<ReportMapMarkerAttachment>
{
    public void Configure(EntityTypeBuilder<ReportMapMarkerAttachment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.HasOne(x => x.Marker)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.MarkerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.MarkerId);
    }
}