using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class BootsOfBravery : MonoBehaviour
{
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

    public void OnDisable() {
        if (GetComponent<Item>().Owner != null) {
            GetComponent<Item>().Owner.afterTakeAction.Remove(AfterOwnerTakeAction);
        }
    }

    private async Task AfterOwnerTakeAction(MoveAction action) {
        if (action is Walk walk) {
            var moveDirection = walk.to.position - walk.from.position;
            var target = walk.to.Shift(moveDirection).GetFigure<Monster>(m => m.Threatening);
            if (target != null) {
                if (!walk.from.Neighbours().Any(n => n.GetFigure<Monster>(m => m.Threatening))) {
                    await GetComponent<Item>().Owner.GetComponent<MovesReserve>().Haste(1);
                }
            }
        }
    }
}
