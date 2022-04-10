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
    public static IEnumerable<T> Single<T>(this T element)
    {
        yield return element;
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
}