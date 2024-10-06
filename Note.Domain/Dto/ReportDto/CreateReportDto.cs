using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Dto.ReportDto
{
    public record CreateReportDto(string Name, string Description, long UserId);
}
