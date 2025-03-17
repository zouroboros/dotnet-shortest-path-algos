using System.Globalization;

namespace GtfsPlanner;

public static class IEnumerables
{
    public static IEnumerable<(TElement, TElement)> PredecessorPairs<TElement>(this IEnumerable<TElement> source)
    {
        var first = true;

        var previousElement = default(TElement);
        
        foreach (var element in source)
        {
            if (first)
            {
                previousElement = element;
                first = false;
            }
            else
            {
                yield return (previousElement!, element);
                previousElement = element;
            }
        }
    }
}