// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Tests;

using Xunit;
using SelTools.Extensions;

#pragma warning disable CA1861

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("asdf", null)]
    [InlineData("0.9428753", null)]
    [InlineData("-123", true)]
    [InlineData("0", false)]
    [InlineData("123", true)]
    [InlineData("no", false)]
    [InlineData("No", false)]
    [InlineData("Off", false)]
    [InlineData("On", true)]
    [InlineData("YeS", true)]
    [InlineData("yes", true)]
    public void FuzzyBoolConversionIsCorrect(string? input, bool? expected)
    {
        // we expect this to fail
        if (expected is null)
        {
            // try fuzzy check
            Assert.Multiple(() =>
            {
                Assert.False(input.TryParseAsFuzzyBool(out var actual));
                Assert.False(actual);
            });

            Assert.Throws<FormatException>(() => input.ParseAsFuzzyBool());
        }
        else
        {
            Assert.Multiple(() =>
            {
                Assert.True(input.TryParseAsFuzzyBool(out var actual));
                Assert.Equal(expected, actual);
            });

            Assert.Equal(expected, input.ParseAsFuzzyBool());
        }
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("TestingToSnakeCase", "testing_to_snake_case")]
    [InlineData("testingToSnakeCase", "testing_to_snake_case")]
    [InlineData("_TrickyToSnakeCase", "_tricky_to_snake_case")]
    [InlineData("_trickyToSnakeCase", "_tricky_to_snake_case")]
    [InlineData("Testing2SnakeCase", "testing2_snake_case")]
    public void SnakeCaseConverter(string input, string expected)
        => Assert.Equal(expected, input.ToSnakeCase());

    public enum Example
    {
        Undefined,
        One,
        Two
    }

    [Theory]
    [InlineData("", Example.Undefined)]
    [InlineData("Undefined", Example.Undefined)]
    [InlineData("One", Example.One)]
    [InlineData("Two", Example.Two)]
    public void ValidateEnumParsing(string input, Example expected)
        => Assert.Equal(expected, input.ParseAsEnum<Example>());

    [Fact]
    public void ValidateEnumParsing2()
        => Assert.Throws<InvalidCastException>(() => "asdf".ParseAsEnum<StringExtensionsTests>());

    [Theory]
    [InlineData("a b c", new[] { "a", "b", "c" })]
    [InlineData("a  b c", new[] { "a", "b", "c" })]
    [InlineData(" a  b c ", new[] { "a", "b", "c" })]
    [InlineData(" a 'b c' d", new[] { "a", "b c", "d" })]
    [InlineData(" a \"b c\" d", new[] { "a", "b c", "d" })]
    [InlineData(" a 'b c\" d", new[] { "a", "'b", "c\"", "d" })]
    [InlineData(" a \"b c' d", new[] { "a", "\"b", "c'", "d" })]
    [InlineData("a '' b", new[] { "a", "", "b" })]
    [InlineData("a '' b", new[] { "a", "b" }, true)]
    [InlineData("a \"\" '' b", new[] { "a", "b" }, true)]
    public void TestTokenization(string input, string[] expected, bool skipEmpties = false)
    {
        Assert.Equal(expected, input.ToQuotedTokens(skipEmpties).ToArray());
    }

    [Fact]
    public void TestTokenizationEmpty()
    {
        Assert.Empty("".ToQuotedTokens().ToArray());
    }
}
