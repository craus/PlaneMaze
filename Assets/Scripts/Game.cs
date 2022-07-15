﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : Singletone<Game>
{
    public Player playerSample;
    public Player player;

    public IEnumerable<Cell> cellOrder;
    public int unlockedCells = (int)1e9;

    public Board boardSample;
    public Board board;

    public Gem gemSample;
    public Gem gem;

    public Transform figureParent;

    public HashSet<(Cell, Cell)> contaminations = new HashSet<(Cell, Cell)>();
    public HashSet<Cell> clearedCells = new HashSet<Cell>();
    public HashSet<Gem> gems = new HashSet<Gem>();

    public void Start() {
        player = Instantiate(playerSample, transform);
        board = Instantiate(boardSample, transform);
        player.figure.savePoint = board.GetCell(Vector2Int.zero);
        player.figure.Move(board.GetCell(Vector2Int.zero));
        Debug.LogFormat("New game started");

        //LockCells();
        //PlaceGem();

        player.figure.location.Dark = false;
        player.figure.location.UpdateCell();
    }

    private void LockCells() {
        cellOrder = Algorithm.Prim(
            start: player.figure.location,
            edges: c => c.Neighbours().Where(c => !c.Wall).Select(c => new Algorithm.Weighted<Cell>(c, UnityEngine.Random.Range(0, 1f))),
            maxSteps: 10000
        ).ToList();
        unlockedCells = 50;
        cellOrder.ForEach((i, c) => {
            c.order = i;
            c.UpdateCell();
        });

        Debug.LogFormat($"Cells: {cellOrder.Count()}");
    }

    public void AfterPlayerMove() {
        player.figure.location.Dark = false;

        foreach (var c in contaminations.ToList()) {
            if (Rand.rndEvent(0.02f) && c.Item2 != player.figure.location && !c.Item1.Locked) {
                c.Item2.Dark = true;
            }
        }

        foreach (var c in clearedCells.ToList()) {
            if (c.figures.Count() == 0) {
                if (Rand.rndEvent(0.01f)) {
                    var gem = Instantiate(gemSample, figureParent);
                    gem.GetComponent<Figure>().Move(c);
                    gems.Add(gem);
                }
            }
        }

        foreach (var g in gems.ToList()) {
            if (Rand.rndEvent(0.02f)) {
                Destroy(g.gameObject);
                gems.Remove(g);
            }
        }
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
