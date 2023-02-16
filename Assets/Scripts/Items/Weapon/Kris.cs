using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Kris : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        var leftTarget = Owner.figure.location.Shift(delta.Relative(1, 1)).GetFigure<Unit>(u => u.Vulnerable);
        if (leftTarget != null) {
            if (await Attack(leftTarget)) {
                return true;
            }
        }
        var rightTarget = Owner.figure.location.Shift(delta.Relative(1, -1)).GetFigure<Unit>(u => u.Vulnerable);
        if (rightTarget != null) {
            if (await Attack(rightTarget)) {
                return true;
            }
        }

        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
