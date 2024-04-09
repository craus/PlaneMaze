using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Figure : MonoBehaviour
{
    public int size = 1;

    [SerializeField] private Cell location;
    public Cell Location => location;

    public Func<Figure, Task> collide = null;
    public Func<Figure, Task> collideEnd = null;

    public List<Func<Board, Board, Task>> afterBoardChange = new List<Func<Board, Board, Task>>();
    public List<Func<Cell, Cell, Task>> afterMove = new List<Func<Cell, Cell, Task>>();
    public List<Func<Cell, Cell, Task>> afterWalk = new List<Func<Cell, Cell, Task>>();

    public void Awake() {
        new ValueTracker<Cell>(() => Location, SetLocation);

        new ValueTracker<bool>(
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

    public async Task<bool> CheckWalk(Vector2Int delta, Func<Cell, bool> free = null) {
        return await TryWalk(delta, free, checkOnly: true);
    }

    public async Task<bool> TryWalk(Vector2Int delta, Func<Cell, bool> free = null, bool checkOnly = false) {
        if (GetComponent<Root>().Current > 0) {
            return false;
        }
        var unit = GetComponent<Unit>();
        free ??= c => unit != null ? c.FreeFor(unit) : c.Free;
        var oldPosition = Location;
        var newPosition = Location.Shift(delta);
        if (OccupiedArea(newPosition).All(free)) {
            if (!checkOnly) {
                await Move(newPosition);
            }
            return true;
        }
        return false;
    }

    public async Task<bool> FakeMove(Vector2Int delta) {
        await Move(Location, fakeMove: Location.Shift(delta));
        return true;
    }

    public IEnumerable<Cell> OccupiedArea(Cell baseCell) {
        if (baseCell == null) {
            yield break;
        }
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                yield return baseCell.Shift(i, j);
            }
        }
    }

    public async Task<bool> Move(Cell newLocation, bool isTeleport = false, Cell fakeMove = null, bool teleportAnimation = false) {
        if (!gameObject.activeSelf) return false;
        if (fakeMove == null && GetComponent<Root>() != null && GetComponent<Root>().Current > 0) return false;
        
        var from = Location;
        var fromBoard = from != null ? from.board : null;
        if (!fakeMove && Location != null) {
            OccupiedArea(Location).ForEach(cell => cell.figures.Remove(this));
        }
        if (newLocation == null) return false;

        location = newLocation;
        var toBoard = Location != null ? Location.board : null;
        transform.SetParent(Location.board.figureParent);
        if (fromBoard != toBoard) {
            await Task.WhenAll(afterBoardChange.Select(listener => listener(fromBoard, toBoard)));
        }

        if (this == null || !gameObject.activeSelf) {
            return true;
        }

        if (!fakeMove && Location != null) {
            OccupiedArea(Location).ForEach(cell => cell.figures.Add(this));
        }

        if (newLocation != null) {
            await UpdateTransform(fakeMove, isTeleport, teleportAnimation);
        }

        if (this == null || !gameObject.activeSelf) {
            return true;
        }

        if (from != Location) {
            var previousArea = OccupiedArea(from);
            var newArea = OccupiedArea(Location);
            var leavingArea = previousArea.Except(newArea);
            var enteringArea = newArea.Except(previousArea);

            foreach (var f in leavingArea.SelectMany(c => c.figures).ToList()) {
                if (f.collideEnd != null) {
                    await f.collideEnd(this);
                }
                if (this == null) {
                    return true;
                }
            }

            foreach (var f in enteringArea.SelectMany(c => c.figures).ToList()) {
                if (f != this && f.collide != null) {
                    await f.collide(this);
                }
                if (this == null) {
                    return true;
                }
            }
        }

        foreach (var t in afterMove.ToList().Select(listener => listener(from, newLocation))) {
            await t;
        }

        return true;
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
