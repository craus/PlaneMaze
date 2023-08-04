using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class ValueTracker<T>
{
    public string name;

    public static int cnt;

    public int sampleCount;

    [SerializeField]
    Stack<TimedValue<T>> track = new Stack<TimedValue<T>>();

    Action<T> setValue;
    Func<T> getValue;
    Func<T, bool> isActual;

    public ValueTracker(Action<T> setValue, Func<T> getValue, IEqualityComparer<T> comparer = null, bool useInitialValue = false, T initialValue = default(T)) 
        : this(setValue, getValue, (v) => (comparer ?? EqualityComparer<T>.Default).Equals(v, getValue()), useInitialValue, initialValue) {
    }

    public ValueTracker(Action<T> setValue, Func<T> getValue, Func<T, bool> isActual, bool useInitialValue = false, T initialValue = default(T)) {
        this.setValue = setValue;
        this.getValue = getValue;
        this.isActual = isActual;
//        TimeManager.instance.onUndo += PerformUndo;
//        TimeManager.instance.onTrack += Track;
//        TimeManager.instance.onPushSampleCount += PushSampleCount;
//        TimeManager.instance.onDrop += new Action(Drop);
        ++cnt;
        name = String.Format("VT-{1}-{0}", cnt, typeof(T));
        //Debug.Log(String.Format("Created valueTracker: {0}", name));
    }

    public void Init(T value = default(T)) {
        if (track.Count == 0) {
            track.Push(new TimedValue<T>(value, float.NegativeInfinity));
            sampleCount = track.Count;
        }
    }

    public void Drop() {
        track = new Stack<TimedValue<T>>();
        sampleCount = track.Count;
        Track();
    }

    void Track() {
        if (track.Count > 0 && isActual(track.Peek().value)) {
            return;
        } 
        //Debug.Log(String.Format("tracker {0} consider not actual: {1}", name, track.Count > 0 ? track.Peek().value : default(T)));
        //track.Push(new TimedValue<T>(getValue(), TimeManager.instance.gameTime));
        sampleCount = track.Count;
    }

    void PerformUndo() {
//        while (track.Count > 1 && track.Peek().time > TimeManager.instance.gameTime + 0.001f) {
//            track.Pop();
//        }
        if (track.Count == 0) {
            Init();
        }
        setValue(track.Peek().value);
        sampleCount = track.Count;
    }

    void PushSampleCount() {
        //TimeManager.instance.totalSampleCount += sampleCount;
    }

    public void SetTrack(List<TimedValue<T>> track) {
        this.track = new Stack<TimedValue<T>>(track);
        this.track.Reverse();
        sampleCount = this.track.Count;
    }
}