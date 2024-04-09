using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BitterMire : MonoBehaviour, IBossGenerator
{
    [SerializeField] private Monster boss;

    public void GenerateBoss(List<Cell> cellOrderList) {
        Game.GenerateFigure(
            cellOrderList.Where(cell => cell.Biome == Library.instance.bitterMire && cell.figures.Count() == 0).Rnd(),
            boss
        );
    }
}
