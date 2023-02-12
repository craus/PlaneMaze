using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Hornet : Monster
{
    public async Task<bool> TryAttack(Vector2Int delta) {
        if (figure.location.GetFigure<PeaceTrap>() != null) {
            return false;
        }

        var leftPosition = figure.location.Shift(delta.Relative(1, 1));
        Debug.LogFormat($"left position is {leftPosition}");
        if (leftPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            await Attack(leftPosition.GetFigure<Unit>());
            return true;
        }

        var rightPosition = figure.location.Shift(delta.Relative(1, -1));
        Debug.LogFormat($"right position is {rightPosition}");
        if (rightPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            await Attack(rightPosition.GetFigure<Unit>());
            return true;
        }

        return false;
    }

    public override async Task Move() {
        var delta = moves.Rnd();
        if (!(await TryAttack(delta))) {
            if (!await figure.TryWalk(delta)) {
                await figure.FakeMove(delta);
            }
        }
    }
}
