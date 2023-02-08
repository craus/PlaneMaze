using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Item : MonoBehaviour
{
    public RectTransform icon;

    [ContextMenu("Pick")]
    public void Pick() {
        icon.SetParent(Inventory.instance.itemsFolder);
        Inventory.instance.items.Add(this);
        GetComponent<Figure>().Move(null, isTeleport: true);

    }
}
