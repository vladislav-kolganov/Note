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
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.Property(report => report.Id).ValueGeneratedOnAdd();
            builder.Property(report => report.Name).IsRequired().HasMaxLength(120);
            builder.Property(report => report.Description).IsRequired().HasMaxLength(2000);
        }
    }
}
