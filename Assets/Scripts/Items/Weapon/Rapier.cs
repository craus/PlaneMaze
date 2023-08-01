using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Rapier : Weapon
{
    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);

    public override IEnumerable<Vector2Int> AttackVectors() {
        yield return new Vector2Int(2, 0);
    }

    public override async Task BeforeAttack(Attack attack) => await Owner.figure.TryWalk(attack.delta);
    public override async Task AfterAttack(Attack attack) {
        await base.AfterAttack(attack);
        if (Owner != null) {
            await Owner.GetComponent<MovesReserve>().Haste(1);
        }
    }

    public override Cell AttackLocation(Vector2Int delta, Unit target) => Owner.figure.location.Shift(delta);

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (!Owner.figure.location.Shift(delta).Free) {
            return false;
        }
        var longTarget = Owner.figure.location.Shift(2 * delta).GetFigure<Unit>(u => u.Vulnerable);
        if (longTarget) {
            return await Attack(delta, longTarget);
        }
        return false;
    }
}
