// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Serialization.Structs;

/// <summary>
/// Endianness refers to the sequential order in which bytes are arranged into larger
/// numerical values when stored in memory or when transmitted over digital links.
/// </summary>
public enum Endianness
{
    /// <summary>
    /// The least significant byte (LSB) value, is at the lowest address.
    /// </summary>
    LittleEndian,

    /// <summary>
    /// The most significant byte (MSB) value, is at the lowest address.
    /// </summary>
    BigEndian
}
