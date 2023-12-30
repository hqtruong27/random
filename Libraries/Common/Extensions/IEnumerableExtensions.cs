namespace Common.Extensions;

public static class IEnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(items);

        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in items)
        {
            action(item);
        }
    }
}
