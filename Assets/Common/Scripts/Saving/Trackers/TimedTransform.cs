using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class TimedTransform : TimedValue<Transform>
{
    public TimedTransform(Transform value, float time)
        : base(value, time) {
    }
}