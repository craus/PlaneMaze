using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UndoManager : Singletone<UndoManager>
{
    public event Action onSave;
    public event Action onLoad;

    public int lastSaveIndex = 0;
    public List<BaseTracker> trackers = new List<BaseTracker>();

    public void CreateRandomStateTracker() {
        var vt = new ValueTracker<UnityEngine.Random.State>(() => UnityEngine.Random.state, v => {
            UnityEngine.Random.state = v;
            //Debug.LogFormat($"Set state: {v.GetHashCode().ToString()}");
            //Debug.LogFormat($"State set to: {UnityEngine.Random.state.GetHashCode().ToString()}");
        });
        vt.toString = v => v.GetHashCode().ToString();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            Debug.LogFormat(UnityEngine.Random.state.GetHashCode().ToString());
        }
    }

    public void Save() {
        lastSaveIndex++;
        Debug.LogFormat($"Save {lastSaveIndex}");
        onSave.Invoke();
        Debug.LogFormat($"Total trackers: {trackers.Count}");
        Debug.LogFormat($"Max track size: {trackers.Max(t => t.TrackSize)}");
        Debug.LogFormat($"Total track size: {trackers.Sum(t => t.TrackSize)}");
    }

    public void Load() {
        onLoad.Invoke();
    }

    public void Undo(int count = 1) {
        ConfirmationManager.instance.DropAllConfirmations();
        if (lastSaveIndex <= 1) return;
        lastSaveIndex -= count;
        Load();
        Debug.LogFormat($"Load {lastSaveIndex}");
    }

    public void ResetTrackers() {
        trackers.Clear();
        onSave = () => { };
        onLoad = () => { };
        lastSaveIndex = 0;
        CreateRandomStateTracker();
        Debug.LogFormat("ResetTrackers end");
    }
}
