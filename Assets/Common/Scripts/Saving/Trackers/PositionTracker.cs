using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PositionTracker : AbstractTracker<Vector3>
{
    protected override ValueTracker<Vector3> CreateTracker() {
        return new Vector3Tracker(
            (v) => transform.localPosition = v,
            () => transform.localPosition
        );
    }
}