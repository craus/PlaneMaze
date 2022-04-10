using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ComponentExtensions
{
    public static List<T> GetComponentsInDirectChildren<T>(this Component component) {
        List<T> result = new List<T>();
        foreach (Transform t in component.transform) {
            result = result.Concat(t.GetComponents<T>().ToList()).ToList();
        }
        return result;
    }

    /// <summary>
    /// Find components in children which are not children of some gameobject with same component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public static IEnumerable<T> GetOuterComponentsInChildren<T>(this Component component) {
        foreach (Transform t in component.transform) {
            var cs = t.GetComponents<T>();
            foreach (T c in cs) {
                yield return c;
            }
            if (cs.Count() == 0) {
                foreach (T c in GetOuterComponentsInChildren<T>(t)) {
                    yield return c;
                }
            }
        }
    }
}