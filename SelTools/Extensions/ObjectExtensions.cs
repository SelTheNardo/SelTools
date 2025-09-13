// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Extensions;

public static class ObjectExtensions
{
    public static void ThrowIfNull<T>(this T obj, string param)
    {
        ArgumentNullException.ThrowIfNull(obj);
    }
}
