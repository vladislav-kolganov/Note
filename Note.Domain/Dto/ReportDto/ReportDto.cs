using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Dto.ReportDto
{                           /*          Создание свойств позиционным синтаксисом        */
    public record ReportDto(long Id, string Name, string Description, string DateCreated); // объект, хранящий ограниченное количество полей, для отправки из одного слоя проиложения в другой

}
