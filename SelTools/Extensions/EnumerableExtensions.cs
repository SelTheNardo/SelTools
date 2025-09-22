using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SelTools.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Python's 'Counter' object for LINQ. Example input: [a, b, c, c] results in {'a':1, 'b':1, 'c': 2}
    /// </summary>
    /// <param name="source"></param>
    /// <param name="comparer"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    // https://stackoverflow.com/a/77061284
    public static Dictionary<TSource, int> ToCounter<TSource>(
        this IEnumerable<TSource> source,
        IEqualityComparer<TSource>? comparer = default) where TSource : notnull
    {
        ArgumentNullException.ThrowIfNull(source);

        Dictionary<TSource, int> dictionary = new(comparer);
        foreach (var item in source)
        {
            CollectionsMarshal.GetValueRefOrAddDefault(dictionary, item, out _)++;
        }

        return dictionary;
    }

    // https://gist.github.com/kekyo/2e0c456f506ec31431f33741608d5230?permalink_comment_id=2196675#gistcomment-2196675
    [SuppressMessage("Performance", "CA1814: Prefer jagged arrays over multidimensional")]
    public static T[,] To2dArray<T>(this IEnumerable<IEnumerable<T>> source)
    {
        var data = source
            .Select(x => x.ToArray())
            .ToArray();

        var res = new T[data.Length, data.Max(x => x.Length)];
        for (var i = 0; i < data.Length; ++i)
        {
            for (var j = 0; j < data[i].Length; ++j)
            {
                res[i, j] = data[i][j];
            }
        }

        return res;
    }

    // Adapted from above To2dArray<T> to output a jagged array
    public static T[][] ToJaggedArray<T>(this IEnumerable<IEnumerable<T>> source)
    {
        var data = source
            .Select(x => x.ToArray())
            .ToArray();

        var res = new T[data.Length][];
        for (int i = 0; i < data.Length; i++)
        {
            res[i] = new T[data[i].Length];
            Array.Copy(data[i], res[i], data[i].Length);
        }

        return res;
    }

    /// <summary>
    /// Split into IEnumerable&lt;List&lt;T&gt;&gt; where each sub-list has at most "chunkLength" elements
    /// </summary>
    /// <param name="source"></param>
    /// <param name="chunkLength"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<List<T>> SplitListBy<T>(this ReadOnlyCollection<T> source, int chunkLength)
    {
        for (var i = 0; i < source.Count; i += chunkLength)
        {
            yield return source.Skip(i).Take(Math.Min(chunkLength, source.Count - i)).ToList();
        }
    }
}
