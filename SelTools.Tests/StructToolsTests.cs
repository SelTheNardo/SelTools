// SPDX-License-Identifier: CC0-1.0

using System.ComponentModel.DataAnnotations;
using SelTools.Serialization.Structs;
using Xunit;

namespace SelTools.Tests;

public class StructToolsTests
{
    [Fact]
    public void ValidDeclaredSize()
    {
        var testStruct = new Valid();
        var testStruct2 = new ValidMissingSerializeFieldAttribute();
        var testStruct3 = new InvalidMissingStructToolsAttribute();

        Assert.Equal(8, StructTools.GetDeclaredSize(testStruct));
        Assert.Equal(7, StructTools.GetDeclaredSize(testStruct2));

        Assert.Throws<InvalidOperationException>(() => StructTools.GetDeclaredSize(testStruct3));
    }

    [Fact]
    public void ValidateDeclaredSize()
    {
        var shouldValidate = new Valid();
        var shouldValidEvenMissingField = new ValidMissingSerializeFieldAttribute();
        var shouldFailToValidate = new InvalidLengthMismatch();
        var shouldFailOnMissingAttribute = new InvalidMissingStructToolsAttribute();
        var shouldFailOnInvalidOffset = new InvalidFieldOffset();

        Assert.Empty(StructTools.GetValidationErrors(shouldValidate));
        Assert.Empty(StructTools.GetValidationErrors(shouldValidEvenMissingField));
        Assert.NotEmpty(StructTools.GetValidationErrors(shouldFailToValidate));
        Assert.NotEmpty(StructTools.GetValidationErrors(shouldFailOnMissingAttribute));
        Assert.NotEmpty(StructTools.GetValidationErrors(shouldFailOnInvalidOffset));
    }

    [Fact]
    public void SerializeStruct()
    {
        var actual = StructTools.Serialize(new Valid());
        var expected = new byte[]
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SerializeAllSupportedTypes()
    {
        var actual = StructTools.Serialize(new ValidAllSupportedTypes());
        var expected = new byte[]
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SerializeAllSupportedTypesBigEndian()
    {
        var actual = StructTools.Serialize(
            obj: new ValidAllSupportedTypes(),
            endianness: Endianness.BigEndian);
        var expected = new byte[]
        {
            0x01, 0x02, 0x03, 0x05, 0x04, 0x07, 0x06, 0x0b, 0x0a, 0x09, 0x08, 0x0f, 0x0e, 0x0d, 0x0c,
            0x17, 0x16, 0x15, 0x14, 0x13, 0x12, 0x11, 0x10, 0x1f, 0x1e, 0x1d, 0x1c, 0x1b, 0x1a, 0x19, 0x18
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SerializeAnUnsupportedType()
    {
        Assert.Throws<InvalidCastException>(() => StructTools.Serialize(new InvalidUnsupportedType()));
    }

    [Fact]
    public void SerializeBrokenStruct()
    {
        Assert.Throws<ValidationException>(() => StructTools.Serialize(new InvalidFieldOffset()));
    }
}
