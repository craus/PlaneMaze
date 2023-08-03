using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Club : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = Owner.figure.location.Shift(delta);
        var target = newPosition.GetFigure<Unit>(u => u.Vulnerable);
        if (target == null) {
            return false;
        }
        if (!Game.CanAttack(Owner, target, this)) {
            return false;
        }
        var attack = Attack(delta, target);
        if (target.Movable && await target.figure.TryWalk(delta)) {
            if (target.alive) {
                await target.GetComponent<MovesReserve>().Freeze(1);
            }
        } else {
            if (Owner.alive) {
                await Owner.GetComponent<MovesReserve>().Freeze(1);
            }
        }
        await attack;
        return true;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
