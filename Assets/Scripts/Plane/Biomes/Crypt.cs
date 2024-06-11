using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crypt : MonoBehaviour, IBossGenerator
{
    [SerializeField] private Lich boss;

    public void GenerateBoss(List<Cell> cellOrderList) {
        Game.GenerateFigure(
            cellOrderList.Where(cell => cell.Biome == Library.instance.crypt && cell.figures.Count() == 0).Rnd(),
            boss
        );
    }
}
