using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class Slippers : MonoBehaviour, IBeforeWalk
{
    public Vector2Int chargeDirection;

    public GameObject activeIcon;

    public void Awake() {
        activeIcon.SetActive(false);
        GetComponent<Item>().afterPick.Add(AfterPick);
        GetComponent<Item>().beforeDrop.Add(BeforeDrop); 
        new ValueTracker<Vector2Int>(() => chargeDirection, v => {
            chargeDirection = v;
        });
    }

    private async Task AfterPick() {
        GetComponent<Item>().Owner.afterTakeAction.Add(AfterOwnerTakeAction);
    }

    private async Task BeforeDrop() {
        GetComponent<Item>().Owner.afterTakeAction.Remove(AfterOwnerTakeAction);
        chargeDirection = Vector2Int.zero;
    }

    public void OnDestroy() {
        if (GetComponent<Item>().Owner != null) {
            GetComponent<Item>().Owner.afterTakeAction.Remove(AfterOwnerTakeAction);
        }
    }

    private async Task AfterOwnerTakeAction(MoveAction action) {
        if (action is FailedMove failedMove) {
            chargeDirection = failedMove.direction;
            activeIcon.SetActive(true);
            return;
        }
        chargeDirection = Vector2Int.zero;
        activeIcon.SetActive(false);
    }

    public async Task<bool> BeforeWalk(Vector2Int delta, int priority) {
        if (priority != 2) {
            return false;
        }
        if (chargeDirection == Vector2Int.zero) {
            return false;
        }
        var result = await GetComponent<Item>().Owner.figure.TryWalk(chargeDirection + delta);
        chargeDirection = Vector2Int.zero;
        return result;
    }
}
