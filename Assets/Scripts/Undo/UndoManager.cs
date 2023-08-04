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

    public void Save() {
        lastSaveIndex++;
        onSave.Invoke();
    }

    public void Load() {
        onLoad.Invoke();
    }

    public void Undo(int count = 1) {
        lastSaveIndex -= count;
        Load();
    }
}
