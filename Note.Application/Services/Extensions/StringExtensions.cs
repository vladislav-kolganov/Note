namespace Note.Application.Services.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Проверяет строку на то, что она пуста или содержит только разделительные символы.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Возвращает false, если строка не пустая или не содержит только разделительные символы, иначе true.</returns>
        public static bool IsNullOrWhiteSpace(this String source)
        {
            return !String.IsNullOrWhiteSpace(source);
        }
    }
}
