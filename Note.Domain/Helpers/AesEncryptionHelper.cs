using System.Security.Cryptography;
using System.Text;

namespace Note.Domain.Helpers;

/// <summary>
/// Вспомогательный класс для AES-256-GCM шифрования.
/// Формат зашифрованных данных: [nonce (12 байт) | ciphertext | tag (16 байт)].
/// </summary>
public static class AesEncryptionHelper
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int KeySize = 32;

    public static string Encrypt(string plaintext, byte[] key)
    {
        if (plaintext == null) return null;
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        var cipherBytes = EncryptBytes(plainBytes, key);
        return Convert.ToBase64String(cipherBytes);
    }

    public static string Decrypt(string ciphertext, byte[] key)
    {
        if (ciphertext == null) return null;
        var cipherBytes = Convert.FromBase64String(ciphertext);
        var plainBytes = DecryptBytes(cipherBytes, key);
        return Encoding.UTF8.GetString(plainBytes);
    }

    public static byte[] EncryptBytes(byte[] plaintext, byte[] key)
    {
        if (plaintext == null) return null;
        if (key == null || key.Length != KeySize)
            throw new ArgumentException($"Ключ должен быть {KeySize} байт.", nameof(key));

        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        var result = new byte[NonceSize + ciphertext.Length + TagSize];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(ciphertext, 0, result, NonceSize, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, NonceSize + ciphertext.Length, TagSize);

        return result;
    }

    public static byte[] DecryptBytes(byte[] ciphertext, byte[] key)
    {
        if (ciphertext == null) return null;
        if (key == null || key.Length != KeySize)
            throw new ArgumentException($"Ключ должен быть {KeySize} байт.", nameof(key));
        if (ciphertext.Length < NonceSize + TagSize)
            throw new ArgumentException("Зашифрованные данные слишком короткие.", nameof(ciphertext));

        var nonce = new byte[NonceSize];
        Buffer.BlockCopy(ciphertext, 0, nonce, 0, NonceSize);

        var tag = new byte[TagSize];
        Buffer.BlockCopy(ciphertext, ciphertext.Length - TagSize, tag, 0, TagSize);

        var encryptedData = new byte[ciphertext.Length - NonceSize - TagSize];
        Buffer.BlockCopy(ciphertext, NonceSize, encryptedData, 0, encryptedData.Length);

        var plaintext = new byte[encryptedData.Length];
        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, encryptedData, tag, plaintext);

        return plaintext;
    }
}
