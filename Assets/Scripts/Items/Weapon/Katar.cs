using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Katar : Weapon
{
    public async Task<bool> TryShortAttack(Vector2Int delta) {
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
        return false;
    }

    public async Task<bool> TryLongAttack(Vector2Int delta) {
        if (!Owner.figure.location.Shift(delta).Free) {
            return false;
        }
        var longTarget = Owner.figure.location.Shift(2 * delta).GetFigure<Unit>(u => u.Vulnerable);
        if (longTarget != null) {
            return await Attack(delta, longTarget, beforeAttack: delta => Owner.figure.TryWalk(delta));
        }
        return false;
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (await TryShortAttack(delta)) {
            return true;
        }
        return await TryLongAttack(delta);
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
