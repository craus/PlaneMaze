using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Item : MonoBehaviour
{
    public RectTransform iconParent;
    public RectTransform icon;
    public ItemSlot slot;

    public void Awake() {
        Game.instance.afterPlayerMove.AddListener(AfterPlayerMove);
    }

    private void AfterPlayerMove() {
        if (Game.instance.player.figure.location == GetComponent<Figure>().location) {
            GetComponent<Figure>().gameObject.SetActive(false);
        } else {
        }
    }

    [ContextMenu("Pick")]
    public void Pick() {
        Inventory.instance.items.Where(item => item.slot == slot).ToList().ForEach(item => item.Drop());

        icon.SetParent(Inventory.instance.itemsFolder);
        Inventory.instance.items.Add(this);
        GetComponent<Figure>().Move(null, isTeleport: true);
    }

    [ContextMenu("Drop")]
    public void Drop() {
        icon.SetParent(iconParent);
        Inventory.instance.items.Remove(this);
        GetComponent<Figure>().Move(Game.instance.player.figure.location, isTeleport: true);
    }
}
