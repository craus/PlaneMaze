using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class TimedFloat : TimedValue<float>
{
    public TimedFloat(float value, float time) : base(value, time) {
    }
}