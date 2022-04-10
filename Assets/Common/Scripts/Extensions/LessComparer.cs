using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LessComparer<T> : IComparer<T>
{
    private Func<T, T, bool> less;

    public LessComparer(Func<T, T, bool> less) {
        this.less = less;
    }

    public int Compare(T x, T y) {
        if (less(x, y)) {
            return -1;
        }
        if (less(y, x)) {
            return 1;
        }
        return 0;
    }
}