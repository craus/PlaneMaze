using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class TransformExtensions
{
    public static void Clear(this Transform transform) {
        transform.Children().ForEach(c => UnityEngine.Object.Destroy(c.gameObject));
    }
}