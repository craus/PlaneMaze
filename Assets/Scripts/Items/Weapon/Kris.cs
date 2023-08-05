using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Kris : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        var leftTarget = Owner.figure.Location.Shift(delta.Relative(1, 1)).GetFigure<Unit>(u => u.Vulnerable);
        if (leftTarget != null) {
            if (await Attack(delta, leftTarget)) {
                return true;
            }
        }
        var rightTarget = Owner.figure.Location.Shift(delta.Relative(1, -1)).GetFigure<Unit>(u => u.Vulnerable);
        if (rightTarget != null) {
            if (await Attack(delta, rightTarget)) {
                return true;
            }
        }

        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
