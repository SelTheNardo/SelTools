// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Tests;

using SelTools.Extensions;
using Xunit;

public class NumericExtensionTests
{
    [Theory]
    [InlineData(0, "0B")]
    [InlineData(999, "999B")]
    [InlineData(1_000, "1.000kB")]
    [InlineData(1_024, "1.024kB")]
    [InlineData(1_000_000, "1.000MB")]
    [InlineData(1_048_576, "1.049MB")]
    [InlineData(1_000_000_000, "1.000GB")]
    [InlineData(1_073_741_824, "1.074GB")]
    [InlineData(1_000_000_000_000, "1.000TB")]
    [InlineData(1_099_511_627_776, "1.100TB")]
    [InlineData(1_000_000_000_000_000, "1.000PB")]
    [InlineData(1_125_899_906_842_624, "1.126PB")]
    [InlineData(1_000_000_000_000_000_000, "1000.000PB")]
    public void Base10FriendlyNameTests(ulong size, string expected)
    {
        Assert.Equal(expected, size.ToFriendlyStorageSize(true));
    }

    [Theory]
    [InlineData(0, "0B")]
    [InlineData(999, "999B")]
    [InlineData(1_000, "1000B")]
    [InlineData(1_024, "1.000KiB")]
    [InlineData(1_000_000, "976.562KiB")]
    [InlineData(1_048_576, "1.000MiB")]
    [InlineData(1_000_000_000, "953.674MiB")]
    [InlineData(1_073_741_824, "1.000GiB")]
    [InlineData(1_000_000_000_000, "931.323GiB")]
    [InlineData(1_099_511_627_776, "1.000TiB")]
    [InlineData(1_000_000_000_000_000, "909.495TiB")]
    [InlineData(1_125_899_906_842_624, "1.000PiB")]
    [InlineData(1_000_000_000_000_000_000, "888.178PiB")]
    public void Base2FriendlyNameTests(ulong size, string expected)
    {
        Assert.Equal(expected, size.ToFriendlyStorageSize());
    }
}
