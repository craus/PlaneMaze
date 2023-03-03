using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class DualDaggers : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        bool result = false;
        var leftTarget = Owner.figure.location.Shift(delta.Relative(1, 1)).GetFigure<Unit>(u => u.Vulnerable);
        var rightTarget = Owner.figure.location.Shift(delta.Relative(1, -1)).GetFigure<Unit>(u => u.Vulnerable);
        return (await Task.WhenAll(Attack(leftTarget), Attack(rightTarget))).Any(b => b);
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
