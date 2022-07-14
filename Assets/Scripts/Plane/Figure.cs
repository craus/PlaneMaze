﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Figure : MonoBehaviour
{
    public Cell location;

    public Cell savePoint;

    public UnityEvent<bool> afterMove;

    public void TryMoveWall(Cell from, Cell to) {
        if (from.fieldCell.wall && !to.fieldCell.wall) {
            from.fieldCell.wall = false;
            from.UpdateCell();
            
            to.fieldCell.wall = true;
            to.UpdateCell();
        }
    }
    public void TryMoveWall(Cell from, Vector2Int delta) {
        TryMoveWall(from, from.Shift(delta));
    }

    private bool TryWalk(Vector2Int delta) {
        var newPosition = location.Shift(delta);
        if (!newPosition.Wall && !newPosition.Locked) {
            //TryMoveWall(location.Shift(delta.RotateLeft()), delta);
            //TryMoveWall(location.Shift(delta.RotateRight()), delta);
            Move(newPosition);
            return true;
        }
        return false;
    }

    private bool Deadend(Cell cell, Vector2Int delta) {
        return 
            !cell.fieldCell.wall && 
            //!cell.Shift(-delta).fieldCell.wall &&
            cell.Shift(delta).fieldCell.wall &&
            cell.Shift(delta.RotateLeft()).fieldCell.wall &&
            cell.Shift(delta.RotateRight()).fieldCell.wall; 
    }

    private bool TrySlip(Vector2Int delta) {
        if (!Deadend(location, delta)) {
            return false;
        }

        var cur = location.Shift(delta);
        for (int i = 0; i < 200; i++) {
            cur = cur.Shift(delta);
            if (Deadend(cur, -delta)) {
                Move(cur);
                return true;
            }
            if (Deadend(cur, delta)) {
                return false;
            }
        }
        return false;
    }

    private void TrySwitch(Cell cell, Vector2Int direction) {
        if (
            cell.Shift(direction.Relative(1, 0)).Wall &&
            cell.Shift(direction.Relative(0, 1)).Wall &&
            cell.Shift(direction.Relative(1, 1)).Wall
        ) {
            cell.Shift(direction.Relative(-2, -2)).Wall ^= true;
        }
    }

    private void TrySwitch(Cell cell) {
        TrySwitch(cell, Vector2Int.up);
        TrySwitch(cell, Vector2Int.right);
        TrySwitch(cell, Vector2Int.left);
        TrySwitch(cell, Vector2Int.down);
    }

    public void TryMove(Vector2Int delta) {
        if (TryWalk(delta)) {
            //TrySwitch(location);
            return;
        }
        //if (TrySlip(delta)) return;

        //var jumpPosition = location.Shift(delta * 2);
        //if (!jumpPosition.fieldCell.wall) {
        //    Move(jumpPosition);
        //    return;
        //}

        Move(location);
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

        if (newPosition.fieldCell.trap) {
            Move(savePoint);
        }

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