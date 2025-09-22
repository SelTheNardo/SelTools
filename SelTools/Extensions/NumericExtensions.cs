// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Extensions;

public static class NumericExtensions
{
    private static readonly string[] sizesBase10 = ["B", "kB", "MB", "GB", "TB", "PB"];
    private static readonly string[] sizesBase2 = ["B", "KiB", "MiB", "GiB", "TiB", "PiB"];

    public static string ToFriendlyStorageSize(this long size, bool useBase10 = false)
    {
        return size < 0
            ? $"-{((ulong)Math.Abs(size)).ToFriendlyStorageSizeInternal(useBase10)}"
            : ((ulong)size).ToFriendlyStorageSizeInternal(useBase10);
    }

    public static string ToFriendlyStorageSize(this ulong size, bool useBase10 = false)
        => ToFriendlyStorageSizeInternal(size, useBase10);

    private static string ToFriendlyStorageSizeInternal(this ulong size, bool useBase10 = false)
    {
        if (size < (useBase10 ? 1000u : 1024u))
        {
            return $"{size}B";
        }

        var exp = useBase10
            ? Math.Log2(size) / Math.Log2(1000)
            : Math.Log2(size) / Math.Log2(1024);

        if (exp > 5)
        {
            exp = 5;
        }

        var front = size / (useBase10 ? Math.Pow(1000, (int)exp) : Math.Pow(1024, (int)exp));

        return $"{front:F3}{(useBase10 ? sizesBase10[(int)exp] : sizesBase2[(int)exp])}";
    }
}
