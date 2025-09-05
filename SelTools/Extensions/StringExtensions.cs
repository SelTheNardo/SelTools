// SPDX-License-Identifier: CC0-1.0

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SelTools.Extensions;

[SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
public static partial class StringExtensions
{
    public static T ParseAsEnum<T>(this string? input)
    {
        var objectType = typeof(T);
        if (!objectType.IsEnum)
        {
            throw new InvalidCastException($"{objectType} is not an enum.");
        }

        if (string.IsNullOrEmpty(input))
        {
            return default!;
        }

        return (T)Enum.Parse(typeof(T), input, true);
    }

    public static string? ToSnakeCase(this string? input)
        => string.IsNullOrEmpty(input)
            ? input
            : SnakeCaseReplacementRegex().Replace(input, "$1_$2").ToLowerInvariant();

    /// <summary>
    /// Compatible with https://yaml.org/type/bool.html values (and also adds null)
    /// </summary>
    /// <param name="input">A string that might be a boolean.</param>
    /// <returns>Boolean processed with extra fuzz.</returns>
    /// <exception cref="FormatException"></exception>
    public static bool ParseAsFuzzyBool(this string? input)
    {
        if (input is null)
        {
            return false;
        }

        if (long.TryParse(input, out var intValue))
        {
            return intValue != 0;
        }

        return input.ToUpperInvariant() switch
        {
            "Y" or "ON" or "YES" or "TRUE" => true,
            "N" or "OFF" or "NO" or "FALSE" or "NULL" => false,
            _ => throw new FormatException($"String '{input}' was not recognized as a valid Boolean.")
        };
    }

    /// <summary>
    /// Compatible with https://yaml.org/type/bool.html values (and also adds null)
    /// </summary>
    /// <param name="input">A string that might be a boolean.</param>
    /// <param name="output">The result of converting the input to a boolean.</param>
    /// <returns>true if successful, otherwise false</returns>
    public static bool TryParseAsFuzzyBool(this string? input, out bool output)
    {
        try
        {
            output = input.ParseAsFuzzyBool();
            return true;
        }
        catch (FormatException)
        {
            output = false;
            return false;
        }
    }

    /// <summary>
    /// Turn a string into a MemoryStream
    /// </summary>
    /// <param name="str"></param>
    /// <returns>A stream that will need disposal.</returns>
    [SuppressMessage("Reliability", "CA2000: Dispose objects before losing scope")]
    public static Stream ToStream(this string str)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(str);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    /// <summary>
    /// Split a string by spaces, allow quoted values. Does not support escaping.
    /// </summary>
    /// <param name="str">The string to split.</param>
    /// <param name="skipEmpty">Do not return empty elements.</param>
    /// <returns>An IEnumerable of strings.</returns>
    public static IEnumerable<string> ToQuotedTokens(this string str, bool skipEmpty = false)
    {
        foreach (Match token in TokenizeWithQuotesRegex().Matches(str))
        {
            // skip actual empties
            if (skipEmpty && (token.Value == "\"\"" || token.Value == "''" || token.Value.Length == 0))
            {
                continue;
            }

            // strip the quotes if we had them
            if ((token.ValueSpan[0] == '\'' && token.ValueSpan[^1] == '\'') || (token.ValueSpan[0] == '"' && token.ValueSpan[^1] == '"'))
            {
                yield return token.ValueSpan[1..^1].ToString();
                continue;
            }

            yield return token.Value;
        }
    }

    /// <summary>
    /// Return a truncated (xor fold) sha256 hash of string. Do not use for cryptographic purposes.
    /// </summary>
    /// <param name="input">String treated as UTF-8 to hash.</param>
    /// <returns>Lowercase (InvariantCulture) 32 character string.</returns>
    public static string GetTruncatedSha256(this string? input)
    {
        var data = string.IsNullOrEmpty(input)
            ? new byte[]
            {
                0xe3, 0xb0, 0xc4, 0x42, 0x98, 0xfc, 0x1c, 0x14,
                0x9a, 0xfb, 0xf4, 0xc8, 0x99, 0x6f, 0xb9, 0x24,
                0x27, 0xae, 0x41, 0xe4, 0x64, 0x9b, 0x93, 0x4c,
                0xa4, 0x95, 0x99, 0x1b, 0x78, 0x52, 0xb8, 0x55
            }
            : SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(Fold32Bytes(data)).ToLowerInvariant();
    }

    private static byte[] Fold32Bytes(byte[] input)
    {
        if (input.Length != 32)
        {
            throw new InvalidDataException("Must be 32 bytes long.");
        }

        var result = new byte[16];
        for (var i = 0; i < 16; i++)
        {
            result[i] = (byte)(input[i] ^ input[i + 16]);
        }

        return result;
    }

    [GeneratedRegex("""
                    "(?:[^"]*)"|'(?:[^']*)'|[^\s]+
                    """)]
    private static partial Regex TokenizeWithQuotesRegex();

    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex SnakeCaseReplacementRegex();
}
