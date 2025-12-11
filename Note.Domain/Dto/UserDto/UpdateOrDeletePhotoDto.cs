namespace Note.Domain.Dto.UserDto;

/// <summary>
/// Дто обновления или удаления фото.
/// </summary>
public record UpdateOrDeletePhotoDto(string Login, string Photo);