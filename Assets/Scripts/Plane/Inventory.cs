using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Singletone<Inventory>
{
    public List<Item> items;
    public RectTransform itemsFolder;

    public T GetItem<T>() where T : class {
        var item = items.FirstOrDefault(i => i.GetComponent<T>() != null);
        if (item == null) {
            return null;
        }
        return item.GetComponent<T>();
    }
}
