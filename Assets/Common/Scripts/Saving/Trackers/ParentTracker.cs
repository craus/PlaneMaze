using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ParentTracker : AbstractTracker<Transform>
{
    protected override ValueTracker<Transform> CreateTracker() {
        return new ValueTracker<Transform>((v) => transform.SetParent(v, worldPositionStays: false), () => transform.parent);
    }
}