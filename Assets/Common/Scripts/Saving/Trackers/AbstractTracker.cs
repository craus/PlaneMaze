using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class AbstractTracker<T> : MonoBehaviour
{
    public ValueTracker<T> tracker;

    public bool useInitialValue = false;
    public T initialValue;

    protected abstract ValueTracker<T> CreateTracker();

    public void Awake() {
        if (tracker != null) {
            return;
        }
        tracker = CreateTracker();
        if (useInitialValue) {
            tracker.Init(initialValue);
        }
    }
}