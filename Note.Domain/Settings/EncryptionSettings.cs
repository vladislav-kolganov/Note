namespace Note.Domain.Settings;

/// <summary>
/// Настройки шифрования сообщений.
/// </summary>
public class EncryptionSettings
{
    public const string DefaultSection = "EncryptionSettings";

    /// <summary>
    /// Base64-строка с 32-байтным (256-битным) ключом для AES-256-GCM.
    /// </summary>
    public string Key { get; set; }
}