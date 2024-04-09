using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BitterMire : MonoBehaviour, IBossGenerator
{
    [SerializeField] private Monster boss;

    private bool BitterMireEmptyCell(Cell cell) => !cell.Wall && cell.Biome == Library.instance.bitterMire && cell.figures.Count() == 0;

    public void GenerateBoss(List<Cell> cellOrderList) {
        Game.GenerateFigure(
            cellOrderList.Where(cell => cell.Vicinity(0, 1, 0, 1).All(BitterMireEmptyCell)).Rnd(),
            boss
        );
    }
}
