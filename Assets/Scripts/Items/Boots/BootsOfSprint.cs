using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class BootsOfSprint : MonoBehaviour
{
    public Vector2Int lastMove;

    public GameObject activeIcon;

    public void Awake() {
        activeIcon.SetActive(false);
        GetComponent<Item>().afterPick.Add(AfterPick);
        GetComponent<Item>().beforeDrop.Add(BeforeDrop);
    }

    private async Task AfterPick() {
        GetComponent<Item>().Owner.afterTakeAction.Add(AfterOwnerTakeAction);
    }

    private async Task BeforeDrop() {
        GetComponent<Item>().Owner.afterTakeAction.Remove(AfterOwnerTakeAction);
        lastMove = Vector2Int.zero;
    }

    public void OnDisable() {
        if (GetComponent<Item>().Owner != null) {
            GetComponent<Item>().Owner.afterTakeAction.Remove(AfterOwnerTakeAction);
        }
    }

    private async Task AfterOwnerTakeAction(MoveAction action) {
        if (action is Walk walk) {
            var moveDirection = walk.to.position - walk.from.position;
            if (moveDirection == lastMove) {
                await GetComponent<Item>().Owner.GetComponent<MovesReserve>().Haste(1);
            } else {
                lastMove = moveDirection;
                activeIcon.SetActive(true);
            }
            return;
        }
        lastMove = Vector2Int.zero;
        activeIcon.SetActive(false);
    }
}
