using Note.Domain.Settings.DbSettings.AdbstractDbSettings;

namespace Note.Domain.Settings.DbSettings;

public class MongoDbSettings : AbstractDbSettings
{
    /// <summary>
    /// База данных.
    /// </summary>
    public string? Database { get; set; }

    /// <summary>
    /// Коллекция в которой хранятся логи.
    /// </summary>
    public string? Collection { get; set; }
}
