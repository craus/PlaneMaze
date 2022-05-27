using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Figure : MonoBehaviour
{
    public Cell location;

    public UnityEvent<bool> afterMove;

    public Vector2Int lastDelta;

    void Wall(Cell cell, bool on) {
        cell.fieldCell.wall = on;
        cell.UpdateCell();
    }

    public bool Walk(Vector2Int delta) {
        var newPosition = location.Shift(delta);
        if (!newPosition.fieldCell.wall) {
            Move(newPosition);
            return true;
        }
        return false;
    }

    public void WalkOrPush(Vector2Int delta) {
        if (Walk(delta)) {
            return;
        }

        var newPosition = location.Shift(delta);
        var jumpPosition = location.Shift(delta * 2);
        if (!jumpPosition.fieldCell.wall) {
            Move(newPosition);
            Wall(newPosition, false);
            Wall(jumpPosition, true);
            return;
        }

        Move(location);
    }

    public void ChargeOrSwap(Vector2Int delta) {
        var cur = location;
        while (!cur.Shift(delta).fieldCell.wall) {
            cur = cur.Shift(delta);
        }
        if (cur == location) {
            Move(cur.Shift(delta));
            Wall(cur, true);
            Wall(cur.Shift(delta), false);
            return;
        }
        Move(cur);
    }

    public void TryMove(Vector2Int delta) {
        Walk(delta);

        //var newPosition = location.Shift(lastDelta == delta ? delta * 2 : delta);

        //if (!newPosition.fieldCell.wall) {
        //    Move(newPosition);
        //    lastDelta = delta;
        //    return;
        //}
    }

    public void Move(Cell newPosition, bool isTeleport = false) {
        if (location != null) {
            location.figures.Remove(this);
        }
        location = newPosition;
        if (location != null) {
            location.figures.Add(this);
        }
        afterMove.Invoke(isTeleport);
        UpdateTransform();
    }

    private void UpdateTransform() {
        transform.position = location.transform.position.Change(z: location.transform.position.z - 1);
    }

    public void OnDestroy() {
        if (location != null) {
            location.figures.Remove(this);
        }
    }
}
