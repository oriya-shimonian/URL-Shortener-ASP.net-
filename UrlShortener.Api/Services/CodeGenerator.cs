using System.Security.Cryptography;
using System.Text;

namespace UrlShortener.Api.Services;

public static class CodeGenerator
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateCode(int length = 6)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        var chars = bytes.Select(b => Alphabet[b % Alphabet.Length]);
        return new string(chars.ToArray());
    }
}
