using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Katar : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        var shortTarget = Owner.figure.location.Shift(delta).GetFigure<Unit>(u => u.Vulnerable);
        if (shortTarget != null) {
            if (!await Attack(delta, shortTarget)) {
                return false;
            }
            if (Owner.alive) {
                if (!await Owner.figure.TryWalk(-delta)) {
                    await Owner.figure.FakeMove(-delta);
                }
            }
            return true;
        }
        var longTarget = Owner.figure.location.Shift(2*delta).GetFigure<Unit>(u => u.Vulnerable);
        if (longTarget != null) {
            var moveToward = await Owner.figure.TryWalk(delta);
            if (!moveToward || !Owner.alive) {
                return false;
            }
            return await Attack(delta, longTarget);
        }
        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
