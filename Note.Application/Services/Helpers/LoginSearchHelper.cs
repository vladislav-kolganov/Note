using System.Text;

namespace Note.Application.Services.Helpers;

public static class LoginSearchHelper
{
    /// <summary>
    /// Нормализует строку для поиска: удаляет лишние символы,
    /// оставляет только буквы и цифры и приводит их к нижнему регистру.
    /// </summary>
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);

        foreach (var ch in value.Trim())
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(char.ToLowerInvariant(ch));
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Вычисляет релевантность логина относительно поисковой строки.
    /// Чем выше значение, тем лучше совпадение.
    /// </summary>
    public static int CalculateScore(string login, string search)
    {
        var normalizedLogin = Normalize(login);
        var normalizedSearch = Normalize(search);

        if (string.IsNullOrEmpty(normalizedLogin) || string.IsNullOrEmpty(normalizedSearch))
        {
            return 0;
        }

        if (normalizedLogin == normalizedSearch)
        {
            return 1000;
        }

        if (normalizedLogin.StartsWith(normalizedSearch))
        {
            return 800;
        }

        var wordParts = SplitLogin(login);

        if (wordParts.Any(x => Normalize(x) == normalizedSearch))
        {
            return 700;
        }

        if (wordParts.Any(x => Normalize(x).StartsWith(normalizedSearch)))
        {
            return 600;
        }

        if (normalizedLogin.Contains(normalizedSearch))
        {
            return 500;
        }

        return 0;
    }

    /// <summary>
    /// Разбивает логин на части по разделителям для последующего анализа.
    /// </summary>
    private static string[] SplitLogin(string login)
    {
        return login
            .Split(new[] { '.', '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }
}