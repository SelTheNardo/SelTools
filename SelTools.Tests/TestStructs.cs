// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Tests;

using SelTools.Serialization.Structs;

// ReSharper disable UnusedMember.Global
// ReSharper disable BuiltInTypeReferenceStyle
#pragma warning disable CS0414 // Field is assigned but its value is never used

[StructTools(Size = 8)]
internal struct Valid
{
    [SerializeField(Offset = 0, Length = 3)]
    public byte[] ThreeByteArray = new byte[] { 0x01, 0x02, 0x03 };

    [SerializeField(Offset = 3, Length = 2)]
    public UInt16 TwoBytesUShort1 = 0x0504;

    [SerializeField(Offset = 5, Length = 2)]
    public UInt16 TwoBytesUShort2 = 0x0706;

    [SerializeField(Offset = 7, Length = 1)]
    public byte OneByte = 0x08;

    public Valid()
    {
    }
}

[StructTools(Size = 8)]
internal struct InvalidLengthMismatch
{
    [SerializeField(Offset = 0, Length = 3)]
    public byte[] ThreeByteArray = new byte[] { 0x01, 0x02, 0x03 };

    [SerializeField(Offset = 3, Length = 2)]
    public UInt16 TypeBytesUShort1 = 0x0504;

    [SerializeField(Offset = 5, Length = 2)]
    public UInt16 TwoBytesUShort2 = 0x0706;

    public InvalidLengthMismatch()
    {
    }
}

internal struct InvalidMissingStructToolsAttribute
{
    [SerializeField(Offset = 0, Length = 3)]
    public byte[] ThreeByteArray = new byte[] { 0x01, 0x02, 0x03 };

    [SerializeField(Offset = 3, Length = 2)]
    public UInt16 TwoBytesUShort1 = 0x0504;

    [SerializeField(Offset = 5, Length = 2)]
    public UInt16 TwoBytesUShort2 = 0x0706;

    public InvalidMissingStructToolsAttribute()
    {
    }
}

[StructTools(Size = 7)]
internal struct ValidMissingSerializeFieldAttribute
{
    [SerializeField(Offset = 0, Length = 3)]
    public byte[] ThreeByteArrayOfBytes = new byte[] { 0x01, 0x02, 0x03 };

    [SerializeField(Offset = 3, Length = 2)]
    public UInt16 TwoByteUshort1 = 0x0504;

    [SerializeField(Offset = 5, Length = 2)]
    public UInt16 TwoByteUshort2 = 0x0706;

    public byte FinalByte = 0x08;

    public ValidMissingSerializeFieldAttribute()
    {
    }
}

[StructTools(Size = 7)]
internal struct InvalidFieldOffset
{
    [SerializeField(Offset = 0, Length = 7)]
    public byte[] ThreeByteArrayOfBytes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

    [SerializeField(Offset = 8, Length = 2)]
    public UInt16 TwoByteUshort = 0x0706;

    public InvalidFieldOffset()
    {
    }
}

[StructTools(Size = 31)]
internal struct ValidAllSupportedTypes
{
    [SerializeField(Offset = 0, Length = 1)]
    public byte[] ByteArray = new byte[] { 0x01 };

    [SerializeField(Offset = 1, Length = 1)]
    public byte SingleByte = 0x02;

    [SerializeField(Offset = 2, Length = 1)]
    public sbyte SingleShortByte = 0x03;

    [SerializeField(Offset = 3, Length = 2)]
    public UInt16 TwoByteUInt16 = 0x0504;

    [SerializeField(Offset = 5, Length = 2)]
    public Int16 TwoByteInt16 = 0x0706;

    [SerializeField(Offset = 7, Length = 4)]
    public UInt32 FourByteUInt32 = 0x0b0a0908;

    [SerializeField(Offset = 11, Length = 4)]
    public Int32 FourByteInt32 = 0x0f0e0d0c;

    [SerializeField(Offset = 15, Length = 8)]
    public UInt64 EightByteUInt64 = 0x1716151413121110;

    [SerializeField(Offset = 23, Length = 8)]
    public Int64 EightByteInt64 = 0x1f1e1d1c1b1a1918;

    public ValidAllSupportedTypes()
    {
    }
}

[StructTools(Size = 32)]
internal struct InvalidUnsupportedType
{
    [SerializeField(Offset = 0, Length = 1)]
    public byte[] ByteArray = new byte[] { 0x01 };

    [SerializeField(Offset = 1, Length = 1)]
    public byte SingleByte = 0x02;

    [SerializeField(Offset = 2, Length = 1)]
    public sbyte SingleShortByte = 0x03;

    [SerializeField(Offset = 3, Length = 2)]
    public UInt16 TwoByteUInt16 = 0x0504;

    [SerializeField(Offset = 5, Length = 2)]
    public Int16 TwoByteInt16 = 0x0706;

    [SerializeField(Offset = 7, Length = 4)]
    public UInt32 FourByteUInt32 = 0x0b0a0908;

    [SerializeField(Offset = 11, Length = 4)]
    public Int32 FourByteInt32 = 0x0f0e0d0c;

    [SerializeField(Offset = 15, Length = 8)]
    public UInt64 EightByteUInt64 = 0x1716151413121110;

    [SerializeField(Offset = 23, Length = 8)]
    public Int64 EightByteInt64 = 0x1f1e1d1c1b1a1918;

    [SerializeField(Offset = 31, Length = 1)]
    public Exception NeverSupported = new InvalidDataException();

    public InvalidUnsupportedType()
    {
    }
}
