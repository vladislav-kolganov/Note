using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Settings.DbSettings.AdbstractDbSettings
{
    /// <summary>
    /// Абстрактный класс настроек БД
    /// </summary>
    public class AbstractDbSettings
    {
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        public string? ConnectionString { get; set; }
    }
}
