﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Hornet : Monster
{
    public override bool Flying => true;

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var leftPosition = figure.location.Shift(delta.Relative(1, 1));
        if (leftPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            if (await Attack(leftPosition.GetFigure<Unit>())) {
                return true;
            }
        }

        var rightPosition = figure.location.Shift(delta.Relative(1, -1));
        if (rightPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            if (await Attack(rightPosition.GetFigure<Unit>())) {
                return true;
            }
        }

        return false;
    }

    protected override async Task MakeMove() {
        for (int i = 0; i < 1; i++) {
            var delta = Moves.Rnd();
            if (!await TryAttack(delta)) {
                if (!await figure.TryWalk(delta)) {
                    await figure.FakeMove(delta);
                }
            }
        }
    }
}
