using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Singletone<Inventory>
{
    public List<Item> items;
    public RectTransform itemsFolder;

    public List<Func<Task>> onPick = new List<Func<Task>>();
    public List<Func<Task>> onDrop = new List<Func<Task>>();

    public void Awake() {
        new ValueTracker<List<Item>>(() => items.ToList(), v => items = v.ToList());
    }

    public T GetItem<T>() where T : class {
        var item = items.FirstOrDefault(i => i.GetComponent<T>() != null);
        if (item == null) {
            return null;
        }
        return item.GetComponent<T>();
    }
}
