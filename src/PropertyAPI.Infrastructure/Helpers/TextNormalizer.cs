using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PropertyAPI.Infrastructure.Helpers;

/// <summary>
/// Helper para normalizar texto eliminando acentos y convirtiendo a minúsculas
/// </summary>
public static class TextNormalizer
{
    /// <summary>
    /// Normaliza un texto eliminando acentos y convirtiendo a minúsculas
    /// </summary>
    public static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        // Normalizar a FormD (descompone caracteres con acentos)
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            // Mantener solo letras, números y espacios (sin marcas diacríticas)
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
    }

    /// <summary>
    /// Crea una expresión regular que busca texto ignorando acentos y mayúsculas/minúsculas
    /// </summary>
    public static string CreateAccentInsensitiveRegex(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return string.Empty;
        }

        var normalized = Normalize(searchTerm);
        var regexPattern = new StringBuilder();

        foreach (var c in normalized)
        {
            switch (c)
            {
                case 'a':
                    regexPattern.Append("[aáàäâã]");
                    break;
                case 'e':
                    regexPattern.Append("[eéèëê]");
                    break;
                case 'i':
                    regexPattern.Append("[iíìïî]");
                    break;
                case 'o':
                    regexPattern.Append("[oóòöôõ]");
                    break;
                case 'u':
                    regexPattern.Append("[uúùüû]");
                    break;
                case 'n':
                    regexPattern.Append("[nñ]");
                    break;
                case 'c':
                    regexPattern.Append("[cç]");
                    break;
                default:
                    // Escapar caracteres especiales de regex
                    if (char.IsLetterOrDigit(c))
                    {
                        regexPattern.Append(Regex.Escape(c.ToString()));
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        regexPattern.Append("\\s+");
                    }
                    else
                    {
                        regexPattern.Append(Regex.Escape(c.ToString()));
                    }
                    break;
            }
        }

        return regexPattern.ToString();
    }
}

