using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Dto.ReportDto
{
    public record UpdateReportDto(long Id, string Name, string Description);
}
