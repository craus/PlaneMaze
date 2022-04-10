using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class RotationTracker : AbstractTracker<Quaternion>
{
    protected override ValueTracker<Quaternion> CreateTracker() {
        return new QuaternionTracker(
            (v) => transform.localRotation = v,
            () => transform.localRotation
        );
    }
}