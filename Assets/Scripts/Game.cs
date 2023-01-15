using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance => GameManager.instance.game;

    public Player playerSample;
    public Player player;

    public IEnumerable<Cell> cellOrder;
    public int unlockedCells = (int)1e9;

    public int time = 0;

    public Board boardSample;
    public Board board;

    public Gem gemSample;
    public Gem gem;

    public Transform figureParent;

    public HashSet<(Cell, Cell)> contaminations = new HashSet<(Cell, Cell)>();
    public HashSet<Cell> clearedCells = new HashSet<Cell>();
    public HashSet<Gem> gems = new HashSet<Gem>();

    public void Start() {
        //player = Instantiate(playerSample, transform);
        board = Instantiate(boardSample, transform);
        //player.figure.savePoint = board.GetCell(Vector2Int.zero);
        //player.figure.Move(board.GetCell(Vector2Int.zero), isTeleport: true);
        Debug.LogFormat("New game started");

        EnumerateCells();
        //PlaceGem();

        //player.figure.location.Dark = false;
        //player.figure.location.UpdateCell();
    }

    private Dictionary<Cell, float> cellPrices = new Dictionary<Cell, float>();

    public float CellPrice(Cell cell) {
        if (!cellPrices.ContainsKey(cell)) {
            cellPrices[cell] = UnityEngine.Random.Range(0, 1f);
        }
        return cellPrices[cell];
    }

    private async Task CommandToContinue() {
        await Task.Delay(1000);
    }

    private async void EnumerateCells() {
        unlockedCells = 0;
        cellOrder = Algorithm.Prim(
            start: board.GetCell(Vector2Int.zero),
            edges: c => c.Neighbours().Where(c => !c.Wall).Select(c => new Algorithm.Weighted<Cell>(c, CellPrice(c))),
            maxSteps: 1000
        );

        int i = 0;
        foreach (Cell c in cellOrder) {

            c.order = i;
            c.UpdateCell();
            await CommandToContinue();

            ++i;
        }

        Debug.LogFormat($"Cells: {cellOrder.Count()}");
        Debug.LogFormat($"Taken Cells Max Price: {cellOrder.Max(CellPrice)}");
    }

    public void Contaminate(Cell cell) {
        if (cell.figures.Any(f => f.GetComponent<Player>())) {
            return;
        }
        if (cell.figures.Any(f => f.GetComponent<Wall>())) {
            if (Rand.rndEvent(0.5f)) {
                return;
            }
            cell.figures.First(f => f.GetComponent<Wall>()).GetComponent<Wall>().Hit();
            return;
        }
        cell.Dark = true;
        foreach (var f in cell.figures.Where(f => f.GetComponent<Building>())) {
            Destroy(f.gameObject);
        }
    }

    public void AfterPlayerMove() {
        player.figure.location.Dark = false;

        foreach (var c in contaminations.ToList()) {
            if (Rand.rndEvent(0.02f) && !c.Item1.Locked) {
                Contaminate(c.Item2);
            }
        }

        foreach (var c in clearedCells.ToList()) {
            if (c.figures.Count() == 0) {
                if (Rand.rndEvent(0.00008f)) {
                    //var gem = Instantiate(gemSample, figureParent);
                    //gem.GetComponent<Figure>().Move(c);
                    //gems.Add(gem);
                    ++player.gems;
                }
            }
        }
        if (Rand.rndEvent(0.008f)) {
            ++player.gems;
        }

        //foreach (var g in gems.ToList()) {
        //    if (Rand.rndEvent(0.02f)) {
        //        Destroy(g.gameObject);
        //        gems.Remove(g);
        //    }
        //}

        time++;
    }

    private void UpdateContamination(Cell from, Cell to) {
        if (from.Dark && !to.Dark) {
            contaminations.Add((from, to));
        } else {
            contaminations.Remove((from, to));
        }
    }

    public void OnCellDarknessChanged(Cell cell) {
        foreach (var n in cell.Neighbours().Where(c => !c.Wall)) {
            UpdateContamination(cell, n);
            UpdateContamination(n, cell);
        }
        if (cell.Dark) {
            clearedCells.Remove(cell);
        } else {
            clearedCells.Add(cell);
        }
        if (unlockedCells < clearedCells.Count() * 2) {
            unlockedCells = clearedCells.Count() * 2;
            cellOrder.Take(unlockedCells).ForEach(c => c.UpdateCell());
        }
    }
}
