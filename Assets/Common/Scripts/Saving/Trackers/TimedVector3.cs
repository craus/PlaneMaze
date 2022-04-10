using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class TimedVector3 : TimedValue<Vector3>
{
    public TimedVector3(Vector3 value, float time) : base(value, time) {
    }
}