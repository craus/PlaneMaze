using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Goblin : Monster
{
    public bool TryAttack(Vector2Int delta) {
        if (figure.location.GetFigure<PeaceTrap>() != null) {
            return false;
        }

        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            Attack(newPosition.GetFigure<Player>());
            return true;
        }
        return false;
    }

    public override async Task Move() {
        var delta = moves.Rnd();
        if (!(await figure.TryWalk(delta))) {
            if (!TryAttack(delta)) {
                await figure.FakeMove(delta);
            }
        }
    }
}
