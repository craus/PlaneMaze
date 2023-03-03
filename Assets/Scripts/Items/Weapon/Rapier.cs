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

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var longTarget = Owner.figure.location.Shift(2 * delta).GetFigure<Unit>(u => u.Vulnerable);
        if (longTarget != null) {
            var moveToward = await Owner.figure.TryWalk(delta);
            if (!moveToward || !Owner.alive) {
                return false;
            }
            if (!await Attack(longTarget)) {
                return false;
            }
            await Owner.GetComponent<MovesReserve>().Haste(1);
            return true;
        }
        return false;
    }
}
