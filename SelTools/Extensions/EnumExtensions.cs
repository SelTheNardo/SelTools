namespace SelTools.Extensions;

public static class EnumExtensions
{
    public static IEnumerable<string> DecodeIntFlags<T>(this Enum e, int flags)
    {
        if (Enum.GetUnderlyingType(typeof(T)) != typeof(int))
        {
            throw new ArgumentException("Only int enums are supported.");
        }

        List<string> result = [];

        foreach (int value in Enum.GetValues(typeof(T)))
        {
            var name = Enum.GetName(typeof(T), value);

            if (string.IsNullOrEmpty(name))
            {
                result.Add("[Unknown Flag]");
                continue;
            }

            if ((value & flags) > 0)
            {
                result.Add(name);
            }
        }

        return result;
    }

    public static IEnumerable<string> DecodeLongFlags<T>(this Enum e, long flags)
    {
        if (Enum.GetUnderlyingType(typeof(T)) != typeof(long))
        {
            throw new ArgumentException("Only long enums are supported.");
        }

        List<string> result = [];

        foreach (long value in Enum.GetValues(typeof(T)))
        {
            var name = Enum.GetName(typeof(T), value);

            if (string.IsNullOrEmpty(name))
            {
                result.Add("[Unknown Flag]");
                continue;
            }

            if ((value & flags) > 0)
            {
                result.Add(name);
            }
        }

        return result;
    }

}
