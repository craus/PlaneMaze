using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class QuaternionExtensions
{
    public static Quaternion Pow(this Quaternion q, int p) {
        Quaternion result = Quaternion.identity;
        for (int i = 0; i < p; i++) {
            result *= q;
        }
        return result;
    }
}