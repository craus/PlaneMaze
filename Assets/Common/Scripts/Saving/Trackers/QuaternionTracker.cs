using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class QuaternionTracker : ValueTracker<Quaternion>
{
    public QuaternionTracker(Action<Quaternion> setValue, Func<Quaternion> getValue)
        : base(setValue, getValue) {
    }
}