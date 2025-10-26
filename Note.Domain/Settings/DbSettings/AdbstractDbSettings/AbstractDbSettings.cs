namespace Note.Domain.Settings.DbSettings.AdbstractDbSettings;

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
