using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public static class CollectionExtensions
{
    public static IEnumerable<T2> SelectAll<T1, T2>(this IEnumerable<T1> collection, Func<T1, T2> selector) {
        return collection.Select(selector).Where(x => x != null);
    }

    public static IEnumerable<T> Single<T>(this T element)
    {
        yield return element;
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, params T[] exceptions) {
        return collection.Except(exceptions.ToList());
    }

    public static IEnumerable<T> Empty<T>() {
        yield break;
    }

    public static IEnumerable<T> Unique<T>(this IEnumerable<T> collection) {
        return new HashSet<T>(collection).ToList();
    }

    public static int IndexOfMin<T>(this IList<T> list, Func<T, float> criteria)
    {
        int answer = 0;
        for (int i = 0; i < list.Count(); i++)
        {
            if (criteria(list[i]) < criteria(list[answer]))
            {
                answer = i;
            }
        }
        return answer;
    }

    // https://stackoverflow.com/questions/13054281/listt-foreach-with-index
    public static void ForEach<T>(this IEnumerable<T> sequence, Action<int, T> action)
    {
        // argument null checking omitted
        int i = 0;
        foreach (T item in sequence)
        {
            action(i, item);
            i++;
        }
    }

    public static bool IsIncreasingSequence(this IEnumerable<int> sequence)
    {
        var initiated = false;
        int current = 0; // assigned value never used
        foreach (var element in sequence)
        {
            if (initiated && element <= current)
            {
                return false;
            }
            current = element;
            initiated = true;
        }
        return true;
    }

    public static T2 First<T, T2>(this IEnumerable<T> collection) where T2 : class {
        return collection.First(el => el is T2) as T2;
    }
}