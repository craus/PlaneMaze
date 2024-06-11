using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Dagger : Weapon
{
    public GameObject iconUncharged;
    public GameObject iconCharged;

    public bool disarmReady = true;

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);

    private void UpdateIcons() {
        iconUncharged.SetActive(!disarmReady);
        iconCharged.SetActive(disarmReady);
    }

    public override async Task AfterAttack(Attack attack) {
        if (this != null && attack.to != null && disarmReady) {
            await attack.to.GetComponent<Disarm>().Gain(1);
        }
        await base.AfterAttack(attack);
    }

    public void Awake() {
        GetComponent<Item>().afterPick.Add(AfterPick);
        GetComponent<Item>().beforeDrop.Add(BeforeDrop);

        new ValueTracker<bool>(() => disarmReady, v => { disarmReady = v; UpdateIcons(); });
    }

    private async Task AfterPick() {
        GetComponent<Item>().Owner.afterTakeAction.Add(AfterOwnerTakeAction);
    }

    private async Task BeforeDrop() {
        GetComponent<Item>().Owner.afterTakeAction.Remove(AfterOwnerTakeAction);
    }

    public void OnDestroy() {
        if (GetComponent<Item>().Owner != null) {
            GetComponent<Item>().Owner.afterTakeAction.Remove(AfterOwnerTakeAction);
        }
    }

    private async Task AfterOwnerTakeAction(MoveAction action) {
        if (action is Walk walk) {
            disarmReady = true;
            UpdateIcons();
        } else {
            disarmReady = false;
            UpdateIcons();
        }
    }
}
