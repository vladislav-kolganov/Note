using Microsoft.Extensions.Logging;
using Note.Application.Resources;
using Note.Domain.Enum;
using Note.Domain.Result;
namespace Note.Application.Services.Helpers;

/// <summary>
/// Хелпер для однотипного логгирования ошибки.
/// </summary>
/// <typeparam name="T">Тип возвращаемого результата в методе.</typeparam>
public static class LogErrorHelper<T>
{
    /// <summary>
    /// Метод логгирования сообщение исключения и создание и возвращаение generic модели с InternalServerError.
    /// </summary>
    /// <param name="message">Сообщение исключения.</param>
    /// <param name="logger">Логгер.</param>
    public static BaseResult<T> LogException(string message, ILogger logger)
    {
        logger.LogError(message);

        return new BaseResult<T>
        {
            ErrorMessage = ErrorMessage.InternalServerError,
            ErrorCode = (int)ErrorCodes.InternalServerError
        };
    }

    /// <summary>
    /// Метод логгирования сообщение исключения и создание и возвращаение generic модели с InternalServerError.
    /// </summary>
    /// <param name="message">Сообщение исключения.</param>
    /// <param name="logger">Логгер.</param>
    public static CollectionResult<T> LogExceptionForCollection(string message, ILogger logger)
    {
        logger.LogError(message);

        return new CollectionResult<T>
        {
            ErrorMessage = ErrorMessage.InternalServerError,
            ErrorCode = (int)ErrorCodes.InternalServerError
        };
    }
}
