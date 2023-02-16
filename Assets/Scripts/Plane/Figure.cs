using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Figure : MonoBehaviour
{
    public Cell location;

    public Cell savePoint;

    public Func<Cell, Figure, Task> collide = (c, f) => Task.CompletedTask;

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

    public async Task<bool> TryWalk(Vector2Int delta, Func<Cell, bool> free = null) {
        free ??= c => c.Free;
        var newPosition = location.Shift(delta);
        if (free(newPosition)) {
            await Move(newPosition);
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

    public async Task<bool> FakeMove(Vector2Int delta) {
        await Move(location, fakeMove: location.Shift(delta));
        return true;
    }

    public async Task Move(Cell newPosition, bool isTeleport = false, Cell fakeMove = null, bool teleportAnimation = false) {
        var from = location;
        if (!fakeMove && location != null) {
            location.figures.Remove(this);
        }
        location = newPosition;

        if (!fakeMove && location != null) {
            location.figures.Add(this);
        }

        if (newPosition != null) {
            await UpdateTransform(fakeMove, isTeleport, teleportAnimation);
        }

        if (from != location) {
            foreach (var f in location.figures.ToList()) {
                await f.collide(from, this);
                if (this == null) {
                    return;
                }
            }
        }
    }

    private async Task UpdateTransform(Cell fakeMove, bool isTeleport, bool teleportAnimation = false) {
        if (isTeleport) {
            if (!teleportAnimation) {
                transform.position = location.transform.position;
                return;
            } else {
                await transform.Zoom(Vector3.zero, 0.1f);
                transform.position = location.transform.position;
                await transform.Zoom(Vector3.one, 0.1f);
                return;
            }
        }
        if (fakeMove == null) {
            await transform.Move(location.transform.position, 0.05f);
        } else {
            await transform.Move(fakeMove.transform.position, 0.05f, endPhase: 0.33f);
            await transform.Move(location.transform.position, 0.05f, startPhase: 0.67f);
        }
    }

    public void OnDestroy() {
        if (location != null) {
            location.figures.Remove(this);
        }
    }
}
