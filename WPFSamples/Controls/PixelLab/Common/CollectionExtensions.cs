using System.Collections.ObjectModel;

namespace WPFSamples.Controls.PixelLab.Common;

using Contracts;

public static class CollectionExtensions
{
    /// <summary>
    ///     Returns a new <see cref="ReadOnlyCollection{TSource}"/> using the
    ///     contents of <paramref name="source"/>.
    /// </summary>
    /// <remarks>
    ///     The contents of <paramref name="source"/> are copied to
    ///     an array to ensure the contents of the returned value
    ///     don't mutate.
    /// </remarks>
    public static ReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
    {
        Contract.Requires(source != null);
        return new ReadOnlyCollection<TSource>(source!.ToArray());
    }

    /// <summary>
    ///     Performs the specified <paramref name="action"/>
    ///     on each element of the specified <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">The sequence to which is applied the specified <paramref name="action"/>.</param>
    /// <param name="action">The action applied to each element in <paramref name="source"/>.</param>
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        Contract.Requires(source != null);
        Contract.Requires(action != null);

        foreach (var item in source!)
        {
            action?.Invoke(item);
        }
    }

    /// <summary>
    ///     If <paramref name="source"/> is null, return an empty <see cref="IEnumerable{TSource}"/>;
    ///     otherwise, return <paramref name="source"/>.
    /// </summary>
    public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source)
    {
        return source ?? [];
    }
}