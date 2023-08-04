using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class FloatTracker : ValueTracker<float>
{
    public FloatTracker(Action<float> setValue, Func<float> getValue)
        : base(setValue, getValue) {
    }
}