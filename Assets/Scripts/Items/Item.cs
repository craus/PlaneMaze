using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Figure))]
public class Item : MonoBehaviour, IExplainable
{
    public RectTransform iconParent;
    public RectTransform icon;
    public Canvas iconCanvas;
    public ItemSlot slot;
    public Image iconImage;

    [Multiline(8)]
    public string description;

    public List<Func<Task>> beforePick = new List<Func<Task>>();
    public List<Func<Task>> beforeDrop = new List<Func<Task>>();

    public List<Func<Task>> afterPick = new List<Func<Task>>();
    public List<Func<Task>> afterDrop = new List<Func<Task>>();

    public GameObject model;

    public virtual bool ShowDescription => description.Length > 0;

    public async Task<bool> AfterFailedWalk(Vector2Int delta, int priority) {
        var activatable = GetComponent<IAfterFailedWalk>();
        if (activatable != null) {
            return await activatable.AfterFailedWalk(delta, priority);
        }
        return false;
    }

    public async Task<bool> BeforeWalk(Vector2Int delta, int priority) {
        var activatable = GetComponent<IBeforeWalk>();
        if (activatable != null) {
            return await activatable.BeforeWalk(delta, priority);
        }
        return false;
    }

    public void Awake() {
        iconCanvas.enabled = false;

        GetComponent<Figure>().collide = async (from, figure) => {
            if (figure == null) {
                return;
            }
            var player = figure.GetComponent<Player>();
            if (player != null && from != GetComponent<Figure>().Location) {
                await Pick();
            }
        };
    }

    public bool Equipped => Inventory.instance.items.Contains(this);

    public bool Compatible(Item other) => GetComponent<Dart>() != null && other.GetComponent<Dart>() != null;

    [ContextMenu("Pick")]
    public async Task Pick() {

        await Task.WhenAll(beforePick.Select(listener => listener()).ToArray());

        if (slot != null) {
            foreach (var item in Inventory.instance.items.Where(item => item.slot == slot).ToList()) {
                if (!item.Compatible(this)) {
                    await item.Drop();
                }
            }
        }

        if (GetComponent<Gem>()) {
            SoundManager.instance.gemPick.Play();
        } else {
            SoundManager.instance.itemPick.Play();
        }

        if (ShowDescription) {
            InfoPanel.instance.Show(this);
        }

        icon.SetParent(Inventory.instance.itemsFolder);
        Inventory.instance.items.Add(this);
        Debug.LogFormat("item is picked");
        Debug.LogFormat($"item owner is {Owner}");
        await GetComponent<Figure>().Move(null, isTeleport: true);
        UpdateModelVisible();

        await Task.WhenAll(afterPick.Select(listener => listener()).ToArray());
        await Task.WhenAll(Inventory.instance.onPick.Select(listener => listener()).ToArray());
    }

    [ContextMenu("Drop")]
    public async Task Drop() {
        await Task.WhenAll(beforeDrop.Select(listener => listener()).ToArray());
        icon.SetParent(iconParent);
        Inventory.instance.items.Remove(this);
        _ = GetComponent<Figure>().Move(Game.instance.player.figure.Location, isTeleport: true);
        UpdateModelVisible();

        await Task.WhenAll(afterDrop.Select(listener => listener()).ToArray());
        await Task.WhenAll(Inventory.instance.onDrop.Select(listener => listener()).ToArray());
    }

    private void UpdateModelVisible() {
        model.SetActive(!Inventory.instance.items.Contains(this));
        model.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(0, 360));
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

    public Unit Owner => Inventory.instance == null ? null : Inventory.instance.items.Contains(this) ? Player.instance : null;

    public string Text => description;

    public Sprite Icon => iconImage.sprite;

    public Color IconColor => iconImage.color;

    public Material IconMaterial => iconImage.material;

    public IExplainable Sample { get; set; }
}
