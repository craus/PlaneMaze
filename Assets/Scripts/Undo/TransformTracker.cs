using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransformTracker : MonoBehaviour
{
    public void Awake() {
        new ValueTracker<Vector3>(
            () => transform.localPosition,
            v => transform.localPosition = v
        );
        new ValueTracker<Quaternion>(
            () => transform.localRotation,
            v => transform.localRotation = v
        );
    }
}
