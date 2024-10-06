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
            builder.Property(report => report.Name).IsRequired().HasMaxLength(200);
            builder.Property(report => report.Description).IsRequired().HasMaxLength(2000);

            builder.HasData(new List<Report>()
            {
                new Report()
                {
                    Id = 1,
                    Name = "testname1",
                    Description = "tesrjigbejrfhibugehjrfiu",
                    UserId = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Report()
                {
                    Id = 2,
                    Name = "testname2",
                    Description = "fvddfgvdfvfvddfvvfd",
                    UserId = 2,
                    CreatedAt = DateTime.UtcNow
                }
            });
        }
    }
}
