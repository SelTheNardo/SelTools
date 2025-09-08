// SPDX-License-Identifier: CC0-1.0

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SelTools.Serialization.Structs;

public static class StructTools
{
    public static byte[] Serialize(object obj, Endianness endianness=Endianness.LittleEndian)
    {
        var errors = GetValidationErrors(obj);
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("\n", errors));
        }

        using var ms = new MemoryStream();

        var structAttr = (StructToolsAttribute)(obj.GetType().GetCustomAttributes(typeof(StructToolsAttribute)).First());
        using var writer = endianness == Endianness.LittleEndian
              ? new BinaryWriter(ms)
              : new BinaryWriterBigEndian(ms);

        var fields = obj
            .GetType()
            .GetFields()
            .Where(f => f.GetCustomAttributes(typeof(SerializeFieldAttribute)).FirstOrDefault() is not null)
            .OrderBy(f =>
                ((SerializeFieldAttribute)f.GetCustomAttributes(typeof(SerializeFieldAttribute)).First()).Offset)
            .ToList();

        foreach (var field in fields)
        {
            var fieldAttr = (SerializeFieldAttribute)field.GetCustomAttributes(typeof(SerializeFieldAttribute)).First();

            if (field.FieldType == typeof(byte))
            {
                writer.Write((byte)(field.GetValue(obj) ?? 0));
            }
            else if (field.FieldType == typeof(byte[]))
            {
                var data = new Span<byte>((byte[])(field.GetValue(obj) ?? new byte[fieldAttr.Length]));
                writer.Write(data[..fieldAttr.Length]);
            }
            else if (field.FieldType == typeof(uint))
            {
                writer.Write((uint)(field.GetValue(obj) ?? 0));
            }
            else if (field.FieldType == typeof(int))
            {
                writer.Write((int)(field.GetValue(obj) ?? 0));
            }
            else if (field.FieldType == typeof(ulong))
            {
                writer.Write((ulong)(field.GetValue(obj) ?? 0));
            }
            else if (field.FieldType == typeof(long))
            {
                writer.Write((long)(field.GetValue(obj) ?? 0));
            }
            else if (field.FieldType == typeof(ushort))
            {
                writer.Write((ushort)(field.GetValue(obj) ?? 0));
            }
            else if (field.FieldType == typeof(short))
            {
                writer.Write((short)(field.GetValue(obj) ?? 0));
            }
            else if (field.FieldType == typeof(sbyte))
            {
                writer.Write((sbyte)(field.GetValue(obj) ?? 0));
            }
            else
            {
                throw new InvalidCastException(
                    $"Unable to serialize type {field.FieldType} on field {field.Name} on {obj.GetType()}");
            }
        }

        return ms.ToArray();
    }

    public static int GetDeclaredSize(object obj)
    {
        var attr = obj.GetType().GetCustomAttributes(typeof(StructToolsAttribute)).FirstOrDefault();
        if (attr is StructToolsAttribute casted)
        {
            return casted.Size;
        }

        throw new InvalidOperationException(
            $"{obj.GetType()} has not been annotated with {nameof(StructToolsAttribute)}");
    }

    public static IReadOnlyCollection<string> GetValidationErrors(object anything)
    {
        var errors = new List<string>();

        var structAttr = anything.GetType().GetCustomAttributes(typeof(StructToolsAttribute)).FirstOrDefault();
        if (structAttr is not StructToolsAttribute castedStructAttr)
        {
            errors.Add($"{anything.GetType()} has not been annotated with {nameof(StructToolsAttribute)}");
            return errors.AsReadOnly();
        }

        var size = 0;
        foreach (var field in anything.GetType().GetFields())
        {
            var fieldAttr = field.GetCustomAttributes(typeof(SerializeFieldAttribute)).FirstOrDefault();
            if (fieldAttr is not SerializeFieldAttribute castedFieldAttr)
            {
                continue;
            }

            if (castedFieldAttr.Offset >= castedStructAttr.Size)
            {
                errors.Add(
                    $"{anything.GetType()} has invalid offset in {typeof(SerializeFieldAttribute)} on field '{field.Name}'");
                continue;
            }

            size += castedFieldAttr.Length;
        }

        if (size != castedStructAttr.Size)
        {
            errors.Add(
                $"{anything.GetType()} declared size ({castedStructAttr.Size}) in {typeof(SerializeFieldAttribute)} does not match annotated fields ({size}).");
        }

        return errors.AsReadOnly();
    }
}
