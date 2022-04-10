using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public static class StringExtensions
{
    public static string Suffix(this string s, int length)
    {
        if (length >= s.Length)
        {
            return s;
        }
        return s.Substring(s.Length - length);
    }

    public static string Join<T>(this IEnumerable<T> list, string separator = "") {
        return string.Join(separator, list);
    }
}