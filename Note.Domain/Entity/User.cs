using Note.Domain.Interfaces;

namespace Note.Domain.Entity;

/// <summary>
/// Модель пользователя.
/// </summary>
public class User : IEntityId<long>
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Последнее посещение полоьзователя.
    /// </summary>
    public DateTime? LastLoginDate { get; set; }

    /// <summary>
    /// Фото пользователя.
    /// </summary>
    public byte[]? Photo { get; set; }

    /// <summary>
    /// Токен пользователя.
    /// </summary>
    public UserToken UserToken { get; set; }

    /// <summary>
    /// Роль пользователя.
    /// </summary>
    public List<Role> Role { get; set; }

    /// <summary>
    /// Дата создания пользователя.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}