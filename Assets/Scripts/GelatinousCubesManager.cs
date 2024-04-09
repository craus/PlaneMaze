using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GelatinousCubesManager : Singletone<GelatinousCubesManager>
{
    public GelatinousCube gelatinousCubeSample;

    Map<Cell, int> gelatinousCubesCounter = new Map<Cell, int>();
    Map<int, HashSet<Cell>> cellsByAdjacentCubesAmount = new Map<int, HashSet<Cell>>(() => new HashSet<Cell>());

    public void Awake() {
        //GetComponentInParent<Game>().afterMonsterMove.Add(async moveNumber => {
        //    cellsByAdjacentCubesAmount[1].ToList().ForEach(cell => {
        //        if (cell.Free && cell.figures.Count == 0) {
        //            Game.GenerateFigure(cell, gelatinousCubeSample);
        //        }
        //    });
        //});
    }

    private void RemoveAdjacentCellRecords(Cell target) {
        if (target != null) {
            target.Vicinity(1).ForEach(cell => cellsByAdjacentCubesAmount[gelatinousCubesCounter[cell]].Remove(cell));
        }
    }

    private void ChangeAdjacentCounters(Cell target, int delta) {
        if (target != null) {
            target.Vicinity(1).ForEach(cell => gelatinousCubesCounter[cell] += delta);
        }
    }

    private void AddAdjacentCellRecords(Cell target) {
        if (target != null) {
            target.Vicinity(1).ForEach(cell => cellsByAdjacentCubesAmount[gelatinousCubesCounter[cell]].Add(cell));
        }
    }

    public void AfterGelatinousCubeMoved(Cell from, Cell to) {
        RemoveAdjacentCellRecords(from);
        RemoveAdjacentCellRecords(to);

        ChangeAdjacentCounters(from, -1);
        ChangeAdjacentCounters(to, 1);

        AddAdjacentCellRecords(from);
        AddAdjacentCellRecords(to);
    }
}
