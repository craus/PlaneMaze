using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GiantFrog : Monster
{
    public override Cell AttackLocation(Vector2Int delta, Unit target) {
        return target.figure.Location;
    }

    protected override async Task MakeMove() {
        for (int i = 0; i < 1; i++) {
            var delta = Helpers.Moves.Rnd();
            var targetCell = figure.Location.Shift(delta);
            if (!targetCell.Wall && await SmartWalk(2*delta)) {
                var player = targetCell.GetFigure<Player>();
                if (player != null) {
                    await Attack(player);
                }
            }
        }
    }
}
