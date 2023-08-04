using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Figure : MonoBehaviour
{
    [SerializeField] private Cell location;
    public Cell Location => location;

    public Cell savePoint;

    public Func<Cell, Figure, Task> collide = null;

    public List<Func<Board, Board, Task>> afterBoardChange = new List<Func<Board, Board, Task>>();
    public List<Func<Cell, Cell, Task>> afterMove = new List<Func<Cell, Cell, Task>>();
    public List<Func<Cell, Cell, Task>> afterWalk = new List<Func<Cell, Cell, Task>>();

    public void Awake() {
        new ValueTracker<Cell>(() => Location, SetLocation);

        ValueTracker<bool> existenceTracker = null;
        existenceTracker = new ValueTracker<bool>(
            () => gameObject.activeSelf, 
            v => gameObject.SetActive(v), 
            defaultValue: false
        );
    }

    private void SetLocation(Cell newPosition) {
        location = newPosition;
        if (location == null) return;
        var toBoard = Location != null ? Location.board : null;
        transform.SetParent(Location.board.figureParent);
        transform.position = location.transform.position;
    }


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
        if (GetComponent<Root>().Current > 0) {
            return false;
        }
        free ??= c => c.Free;
        var oldPosition = Location;
        var newPosition = Location.Shift(delta);
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
        if (!Deadend(Location, delta)) {
            return false;
        }

        var cur = Location.Shift(delta);
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
        await Move(Location, fakeMove: Location.Shift(delta));
        return true;
    }

    public async Task Move(Cell newPosition, bool isTeleport = false, Cell fakeMove = null, bool teleportAnimation = false) {
        var from = Location;
        var fromBoard = from != null ? from.board : null;
        if (!fakeMove && Location != null) {
            Location.figures.Remove(this);
        }
        if (newPosition == null) return;

        location = newPosition;
        var toBoard = Location != null ? Location.board : null;
        transform.SetParent(Location.board.figureParent);
        if (fromBoard != toBoard) {
            await Task.WhenAll(afterBoardChange.Select(listener => listener(fromBoard, toBoard)));
        }
        await Task.WhenAll(afterMove.Select(listener => listener(from, Location)));

        if (!fakeMove && Location != null) {
            Location.figures.Add(this);
        }

        if (newPosition != null) {
            await UpdateTransform(fakeMove, isTeleport, teleportAnimation);
        }

        if (this == null) {
            return;
        }

        if (from != Location) {
            foreach (var f in Location.figures.ToList()) {
                if (f != this && f.collide != null) {
                    Game.Debug($"Figure {gameObject} collides with figure {f.gameObject}");
                    await f.collide(from, this);
                }
                if (this == null) {
                    return;
                }
            }
        }
    }

    private async Task UpdateTransform(Cell fakeMove, bool isTeleport, bool teleportAnimation = false) {
        if (isTeleport) {
            if (!teleportAnimation) {
                transform.position = Location.transform.position;
                return;
            } else {
                await transform.Zoom(Vector3.zero, 0.05f);
                if (this == null) {
                    return;
                }
                transform.position = Location.transform.position;
                await transform.Zoom(Vector3.one, 0.05f);
                return;
            }
        }
        if (fakeMove == null) {
            await transform.Move(Location.transform.position, 0.05f);
        } else {
            await transform.Move(fakeMove.transform.position, 0.05f, endPhase: 0.33f);
            if (this == null) {
                return;
            }
            await transform.Move(Location.transform.position, 0.05f, startPhase: 0.67f);
        }
    }

    public void OnDestroy() {
        if (Location != null) {
            Location.figures.Remove(this);
        }
    }

    public override string ToString() => $"{gameObject.name} at ({Location.position.x}, {Location.position.y})";
}
