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

    public ValueTracker(Func<T> getter, Action<T> setter, T defaultValue)
        : this(getter, setter, EqualityComparer<T>.Default
    ) {
        track.Add((int.MinValue, defaultValue));
    }

    public ValueTracker(Func<T> getter, Action<T> setter)
        : this(getter, setter, EqualityComparer<T>.Default) {
    }

    public ValueTracker(Func<T> getter, Action<T> setter, EqualityComparer<T> equals) {
        this.getter = getter;
        this.setter = setter;
        this.equals = equals;

        UndoManager.instance.onSave += Save;
        UndoManager.instance.onLoad += Load;

        Save();
    }

    public void Save() {
        var currentValue = getter();
        if (track.Count == 0 || !equals.Equals(currentValue, track.Last().value)) {
            track.Add((UndoManager.instance.lastSaveIndex, currentValue));
        }
    }

    public void Load() {
        while (track.Count >= 2 && track.Last().index > UndoManager.instance.lastSaveIndex) {
            track.RemoveAt(track.Count - 1);
        }
        setter(track.Last().value);
    }
}
