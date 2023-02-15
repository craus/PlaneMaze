using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Harpy : Monster
{
    public override bool Flying => true;

    public async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = figure.location.Shift(2*delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            return await Attack(newPosition.GetFigure<Player>());
        }
        return false;
    }

    protected override async Task MakeMove() {
        var delta = moves.Rnd();
        if (!await TryAttack(delta)) {
            if (!await figure.TryWalk(delta)) {
                await figure.FakeMove(delta);
            }
        }
    }
}
