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
        var leftTargetCell = Owner.figure.Location.Shift(delta.Relative(1, 1));
        var rightTargetCell = Owner.figure.Location.Shift(delta.Relative(1, -1));
        var leftTarget = leftTargetCell.GetFigure<Unit>(u => u.Vulnerable);
        var rightTarget = rightTargetCell.GetFigure<Unit>(u => u.Vulnerable);
        return (await Task.WhenAll(
            Attack(delta, leftTarget, leftTargetCell), 
            Attack(delta, rightTarget, rightTargetCell)
        )).Any(b => b);
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
