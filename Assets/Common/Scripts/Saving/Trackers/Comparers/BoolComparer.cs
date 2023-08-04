using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BoolComparer : IEqualityComparer<bool>
{
    public bool Equals(bool a, bool b) {
        return a == b;
    }

    public int GetHashCode(bool obj) {
        return obj.GetHashCode();
    }
}