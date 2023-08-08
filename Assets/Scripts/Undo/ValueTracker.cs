using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ValueTracker<T> : BaseTracker
{
    public List<(int index, T value)> track = new List<(int, T)>();

    public Func<T> getter;
    public Action<T> setter;
    public EqualityComparer<T> equals;

    public bool verbose = false;
    public Func<T, string> toString;

    public ValueTracker(Func<T> getter, Action<T> setter, T defaultValue)
        : this(getter, setter, EqualityComparer<T>.Default, defaultValue) {
    }

    public ValueTracker(Func<T> getter, Action<T> setter)
        : this(getter, setter, EqualityComparer<T>.Default, getter()) {
    }

    public ValueTracker(Func<T> getter, Action<T> setter, EqualityComparer<T> equals, T defaultValue) {
        this.getter = getter;
        this.setter = setter;
        this.equals = equals;

        UndoManager.instance.onSave += Save;
        UndoManager.instance.onLoad += Load;
        if (DebugManager.verbose) {
            DebugManager.LogFormat($"Subscribed to events: {UndoManager.instance.GetInstanceID()}");
        }

        track.Insert(0, (int.MinValue, defaultValue));
    }

    public string TrackString => track.ExtToString(elementToString: (p) => $"({p.index}: {toString(p.value)})");

    public void Save() {
        if (verbose) {
            Debug.LogFormat($"Tracker saving: {TrackString}");
        }
        var currentValue = getter();
        if (track.Count == 0 || !equals.Equals(currentValue, track.Last().value)) {
            track.Add((UndoManager.instance.lastSaveIndex, currentValue));
            if (verbose) {
                Debug.LogFormat($"Tracker saved: {TrackString}");
            }
        }
    }

    public void Load() {
        while (track.Count >= 2 && track.Last().index > UndoManager.instance.lastSaveIndex) {
            track.RemoveAt(track.Count - 1);
        }
        setter(track.Last().value);
    }
}
