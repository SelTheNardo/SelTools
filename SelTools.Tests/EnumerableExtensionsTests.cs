// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Tests;

using System.Diagnostics.CodeAnalysis;
using SelTools.Extensions;
using Xunit;

public class EnumerableExtensionsTests
{
    [Fact]
    public void ValidateCounterCounts()
    {
        var input = "abccdde";
        var expected = new Dictionary<string, int>
        {
            { "a", 1 },
            { "b", 1 },
            { "c", 2 },
            { "d", 2 },
            { "e", 1 },
        };

        var actual = input.ToList().Select(s => $"{s}").ToCounter();

        Assert.Equal(expected, actual);
    }

    [Fact]
    [SuppressMessage("Performance", "CA1814: Prefer jagged arrays over multidimensional")]
    public void CreatesSquareArray()
    {
        var input = new List<List<char>>();
        input.Add(['a', 'b', 'c', 'd']);
        input.Add(['b', 'c', 'd', 'a']);
        input.Add(['c', 'd', 'a', 'b']);
        input.Add(['d', 'a', 'b', 'c']);
        var expected = new[,]
        {
            { 'a', 'b', 'c', 'd' },
            { 'b', 'c', 'd', 'a' },
            { 'c', 'd', 'a', 'b' },
            { 'd', 'a', 'b', 'c' },
        };

        var actual = input.To2dArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CreateJaggedArray()
    {
        var input = new List<List<char>>();
        input.Add(['a', 'b', 'c']);
        input.Add(['b', 'c', 'd', 'a']);
        input.Add(['c', 'd']);
        input.Add(['d', 'a', 'b', 'c']);
        var expected = new char[][]
        {
            ['a', 'b', 'c'],
            ['b', 'c', 'd', 'a'],
            ['c', 'd'],
            ['d', 'a', 'b', 'c'],
        };

        var actual = input.ToJaggedArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ValidateSplitList()
    {
        List<char> input =
        [
            'a', 'b', 'c', 'd',
            'e', 'f', 'g', 'h',
            'i', 'j', 'k', 'l',
            'm', 'n'
        ];

        List<List<char>> expected =
        [
            ['a', 'b', 'c', 'd'],
            ['e', 'f', 'g', 'h'],
            ['i', 'j', 'k', 'l'],
            ['m', 'n']
        ];
        List<List<char>> expected2 =
        [
            ['a', 'b', 'c', 'd', 'e', 'f', 'g'],
            ['h', 'i', 'j', 'k', 'l', 'm', 'n'],
        ];

        var actual = input.AsReadOnly().SplitListBy(4).ToList();
        var actual2 = input.AsReadOnly().SplitListBy(7).ToList();

        Assert.Equal(expected, actual);
        Assert.Equal(expected2, actual2);
    }
}
