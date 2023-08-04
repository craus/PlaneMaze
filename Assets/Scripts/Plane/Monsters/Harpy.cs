using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Harpy : Monster
{
    public override bool Flying => true;

    public override async Task BeforeAttack(Vector2Int delta) {
        await figure.TryWalk(delta);
    }

    public override async Task AfterAttack(Vector2Int delta) {
        await figure.TryWalk(-delta);
    }

    public override Cell AttackLocation(Vector2Int delta, Unit target) => figure.Location.Shift(delta);

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (!figure.Location.Shift(delta).Free) {
            return false;
        }
        var newPosition = figure.Location.Shift(2*delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            return await Attack(newPosition.GetFigure<Player>(), delta);
        }
        return false;
    }

    protected override async Task MakeMove() {
        var delta = Moves.Rnd();
        if (await TryAttack(delta)) {
            return;
        }
        if (await figure.TryWalk(delta)) {
            return;
        } 
        await figure.FakeMove(delta);
    }
}
