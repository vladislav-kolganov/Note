using Note.Domain.Settings.DbSettings.AdbstractDbSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Settings.DbSettings
{
    /// <summary>
    /// Настройки Постргесс БД
    /// </summary>
    public class PostgresSettings : AbstractDbSettings
    {
        /// <summary>
        /// Схема
        /// </summary>
        public string? Schema { get; set; }
    }
}
