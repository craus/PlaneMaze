using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Figure))]
public class Item : MonoBehaviour
{
    public RectTransform iconParent;
    public RectTransform icon;
    public Canvas iconCanvas;
    public ItemSlot slot;

    public List<Func<Task>> onPick = new List<Func<Task>>();
    public List<Func<Task>> onDrop = new List<Func<Task>>();

    public GameObject model;

    public async Task<bool> AfterFailedWalk(Vector2Int delta) {
        var weapon = GetComponent<Weapon>();
        if (weapon) {
            return await weapon.AfterFailedWalk(delta);
        }
        return false;
    }

    public async Task<bool> BeforeWalk(Vector2Int delta) {
        var weapon = GetComponent<Weapon>();
        if (weapon) {
            return await weapon.BeforeWalk(delta);
        }
        return false;
    }

    public void Awake() {
        iconCanvas.enabled = false;

        GetComponent<Figure>().collide = async (from, figure) => {
            var player = figure.GetComponent<Player>();
            if (player != null && from != GetComponent<Figure>().location) {
                Pick();
            }
        };
    }

    public bool Equipped => Inventory.instance.items.Contains(this);

    [ContextMenu("Pick")]
    public void Pick() {
        if (slot != null) {
            Inventory.instance.items.Where(item => item.slot == slot).ToList().ForEach(item => item.Drop());
        }

        icon.SetParent(Inventory.instance.itemsFolder);
        Inventory.instance.items.Add(this);
        Debug.LogFormat("item is picked");
        Debug.LogFormat($"item owner is {Owner}");
        _ = GetComponent<Figure>().Move(null, isTeleport: true);
        UpdateModelVisible();

        Task.WaitAll(onPick.Select(listener => listener()).ToArray());
        Task.WaitAll(Inventory.instance.onPick.Select(listener => listener()).ToArray());
    }

    [ContextMenu("Drop")]
    public void Drop() {
        icon.SetParent(iconParent);
        Inventory.instance.items.Remove(this);
        _ = GetComponent<Figure>().Move(Game.instance.player.figure.location, isTeleport: true);
        UpdateModelVisible();

        Task.WaitAll(onDrop.Select(listener => listener()).ToArray());
        Task.WaitAll(Inventory.instance.onDrop.Select(listener => listener()).ToArray());
    }

    private void UpdateModelVisible() {
        model.SetActive(!Inventory.instance.items.Contains(this));
    }

    public void OnDestroy() {
        if (Inventory.instance == null) {
            return;
        }
        Inventory.instance.items.Remove(this);
        if (icon != null) {
            Destroy(icon.gameObject);
        }
    }

    public Unit Owner => Inventory.instance.items.Contains(this) ? Player.instance : null;
}
