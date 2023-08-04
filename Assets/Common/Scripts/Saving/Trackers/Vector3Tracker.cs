using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class Vector3Tracker : ValueTracker<Vector3>
{
    public Vector3Tracker(Action<Vector3> setValue, Func<Vector3> getValue)
        : base(setValue, getValue) {
    }
}