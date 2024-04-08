using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inferno : MonoBehaviour, IBossGenerator
{
    [SerializeField] private Archdevil boss;

    public void GenerateBoss(List<Cell> cellOrderList) {
        Game.GenerateFigure(
            cellOrderList.Where(cell => cell.Biome == Library.instance.inferno && cell.figures.Count() == 0).Rnd(),
            boss
        );
    }
}
