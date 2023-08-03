using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class TaskExtensions
{
    public async static Task<bool> AnyInSequence(this IEnumerable<Task<bool>> tasks) {
        foreach (var task in tasks) {
            if (await task) return true;
        }
        return false;
    }
}