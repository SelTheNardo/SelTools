// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Serialization.Structs;

[AttributeUsage(AttributeTargets.Struct)]
public sealed class StructToolsAttribute : Attribute
{
    public int Size { get; init; }
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class SerializeFieldAttribute : Attribute
{
    public int Offset { get; init; }
    public int Length { get; init; }
}
