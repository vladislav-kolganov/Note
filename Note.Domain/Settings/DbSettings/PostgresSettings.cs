using Note.Domain.Settings.DbSettings.AdbstractDbSettings;

namespace Note.Domain.Settings.DbSettings
{
    /// <summary>
    /// Настройки Постргесс БД.
    /// </summary>
    public class PostgresSettings : AbstractDbSettings
    {
        /// <summary>
        /// Схема.
        /// </summary>
        public string? Schema { get; set; }
    }
}
