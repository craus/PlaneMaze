using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class BoolTracker : ValueTracker<bool>
{
    public BoolTracker(Action<bool> setValue, Func<bool> getValue)
        : base(setValue, getValue) {
    }
}