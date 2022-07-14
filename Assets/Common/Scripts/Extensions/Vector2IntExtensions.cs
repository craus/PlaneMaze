using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Vector2IntExtensions
{
    public static Vector2Int RotateLeft(this Vector2Int v) {
        return new Vector2Int(v.y, -v.x);
    }

    public static Vector2Int Relative(this Vector2Int v, int x, int y) {
        return v * x + v.RotateLeft() * y;
    }

    public static Vector2Int Relative(this Vector2Int v, Vector2Int relation) {
        return v * relation.x + v.RotateLeft() * relation.y;
    }

    public static Vector2Int RotateRight(this Vector2Int v) {
        return new Vector2Int(-v.y, v.x);
    }
}