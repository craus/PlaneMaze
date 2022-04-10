using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Vector3Comparer : IEqualityComparer<Vector3>
{
    public bool Equals(Vector3 a, Vector3 b) {
        return a == b;
    }

    public int GetHashCode(Vector3 obj) {
        return obj.GetHashCode();
    }
}