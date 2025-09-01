// SPDX-License-Identifier: CC-BY-SA-3.0
// https://stackoverflow.com/a/32544916

using System.Diagnostics.CodeAnalysis;

namespace SelTools.Extensions;

[SuppressMessage("Performance", "CA1859: Use concrete types when possible for improved performance")]
public static class CombinatoricsExtensions
{
    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> enumerable)
    {
        var array = enumerable as T[] ?? enumerable.ToArray();

        var factorials = Enumerable.Range(0, array.Length + 1)
            .Select(PrivateFactorial)
            .ToArray();

        for (var i = 0L; i < factorials[array.Length]; i++)
        {
            var sequence = GenerateSequence(i, array.Length - 1, factorials);

            yield return GeneratePermutation(array, sequence);
        }
    }

    private static T[] GeneratePermutation<T>(T[] array, IReadOnlyList<int> sequence)
    {
        var clone = (T[])array.Clone();

        for (int i = 0; i < clone.Length - 1; i++)
        {
            Swap(ref clone[i], ref clone[i + sequence[i]]);
        }

        return clone;
    }

    private static int[] GenerateSequence(long number, int size, IReadOnlyList<long> factorials)
    {
        var sequence = new int[size];

        for (var j = 0; j < sequence.Length; j++)
        {
            var facto = factorials[sequence.Length - j];

            sequence[j] = (int)(number / facto);
            number = (int)(number % facto);
        }

        return sequence;
    }

    private static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }

    private static long PrivateFactorial(int n)
    {
        long result = n;

        for (int i = 1; i < n; i++)
        {
            result = result * i;
        }

        return result;
    }

    public static ulong Factorial(uint n) => Enumerable.Range(1, (int)n).Aggregate(1ul, (accumulator, v) => accumulator * (uint)v);
}
