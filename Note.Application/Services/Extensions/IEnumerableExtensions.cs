namespace Note.Application.Services.Extensions;

public static class IEnumerableExtensions
{
    /// <summary>
    /// Проверяет последовательность на null и пустоту
    /// </summary>
    /// <typeparam name="T">Тип последовательности.</typeparam>
    /// <param name="source">Последовательность.</param>
    /// <returns>Если последовательность содержит элементы, вернет true, иначе false.</returns>
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source != null && source.Any();
    }

    /// <summary>
    /// Проверяет последовательность на null и пустоту
    /// </summary>
    /// <typeparam name="T">Тип последовательности.</typeparam>
    /// <param name="source">Последовательность.</param>
    /// <returns>Если последовательность не содержит элементы, вернет true, иначе false.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null && !source.Any();
    }
}
