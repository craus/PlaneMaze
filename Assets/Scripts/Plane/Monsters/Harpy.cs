using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Harpy : Monster
{
    public override bool Flying => true;

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            if (await Attack(newPosition.GetFigure<Player>())) {
                await figure.TryWalk(-delta);
            }
        }
        return false;
    }

    protected override async Task MakeMove() {
        var delta = moves.Rnd();
        if (await figure.TryWalk(delta)) {
            await TryAttack(delta);
        } else {
            await figure.FakeMove(delta);
        }
    }
}
