// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Serialization.Structs;

using System.Text;

internal sealed class BinaryWriterBigEndian : BinaryWriter
{
    public BinaryWriterBigEndian(Stream output) : base(output, Encoding.UTF8, false)
    {
    }

    public override void Write(ulong value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        base.Write(BitConverter.ToUInt64(bytes));
    }

    public override void Write(uint value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        base.Write(BitConverter.ToUInt32(bytes));
    }

    public override void Write(ushort value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        base.Write(BitConverter.ToUInt16(bytes));
    }

    public override void Write(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        base.Write(BitConverter.ToInt64(bytes));
    }

    public override void Write(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        base.Write(BitConverter.ToInt32(bytes));
    }

    public override void Write(short value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Reverse(bytes);
        base.Write(BitConverter.ToInt16(bytes));
    }
}
