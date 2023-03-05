using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class SpikeBoots : MonoBehaviour
{
    public int damage = 1;

    public void Awake() {
        GetComponent<Item>().afterPick.Add(AfterPick);
        GetComponent<Item>().beforeDrop.Add(BeforeDrop);
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
            var moveDirection = walk.to.position - walk.from.position;
            var target = walk.to.Shift(moveDirection).GetFigure<Monster>(m => m.Vulnerable);
            if (target != null) {
                SoundManager.instance.spikeBootsAttack.Play();
                await target.GetComponent<Health>().Hit(damage);
            }
        }
    }
}
