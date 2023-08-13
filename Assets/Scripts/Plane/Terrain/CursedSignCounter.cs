using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Game))]
public class CursedSignCounter : MonoBehaviour
{
    public static int Max = 2;

    public int cursedSignCount = 0;

    public virtual void Awake() {
        new ValueTracker<int>(() => cursedSignCount, v => cursedSignCount = v);
    }
}
