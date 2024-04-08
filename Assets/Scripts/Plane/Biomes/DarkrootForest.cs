using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DarkrootForest : MonoBehaviour, IBossGenerator
{
    public Fog fog;

    public Witch witch;
    public Sister sister;

    public void GenerateBoss(List<Cell> cellOrderList) {
        var witch = Game.GenerateFigure(
            cellOrderList.Where(cell => cell.Biome == Library.instance.darkrootForest && cell.figures.Count() == 0).Rnd(),
            this.witch
        );
        var sister = Game.GenerateFigure(
            cellOrderList.Where(cell => cell.Biome == Library.instance.darkrootForest && cell.figures.Count() == 0).Rnd(),
            this.sister
        );

        witch.witch = witch;
        witch.sister = sister;
        sister.witch = witch;
        sister.sister = sister;
    }
}
