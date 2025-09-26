// SPDX-License-Identifier: CC0-1.0

using System.Diagnostics.CodeAnalysis;
using SelTools.Extensions;
using Xunit;

namespace SelTools.Tests;

public class EnumExtensionsTests
{
    [Fact]
    public void ValidateIntEnumDecoding()
    {
        var input = TestEnumInt.Flag1 | TestEnumInt.Flag3;
        List<string> expected = ["Flag1", "Flag3"];
        var actual = input.DecodeIntFlags<TestEnumInt>((int)input);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ValidateLongEnumDecoding()
    {
        var input = TestEnumLong.Flag1 | TestEnumLong.Flag3;
        List<string> expected = ["Flag1", "Flag3"];
        var actual = input.DecodeLongFlags<TestEnumLong>((long)input);

        Assert.Equal(expected, actual);
    }

    [SuppressMessage("Design", "CA1028: Enum Storage should be Int32")]
    [Flags]
    public enum TestEnumInt
    {
        None = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4
    }

    [SuppressMessage("Design", "CA1028: Enum Storage should be Int32")]
    [Flags]
    public enum TestEnumLong : long
    {
        None = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4
    }
}
