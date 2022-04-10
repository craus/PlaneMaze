using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class FloatComparer : IEqualityComparer<float>
{
    public bool Equals(float a, float b) {
        return a == b;
    }

    public int GetHashCode(float obj) {
        return obj.GetHashCode();
    }
}