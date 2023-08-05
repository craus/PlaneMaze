using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Katar : Weapon
{
    public async Task<bool> TryShortAttack(Vector2Int delta) {
        var shortTarget = Owner.figure.Location.Shift(delta).GetFigure<Unit>(u => u.Vulnerable);
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
        if (!Owner.figure.Location.Shift(delta).Free) {
            Game.Debug("Katar failed long attack: no space to lunge");
            return false;
        }
        var longTarget = Owner.figure.Location.Shift(2 * delta).GetFigure<Unit>(u => u.Vulnerable);
        if (longTarget != null) {
            return await Attack(delta, longTarget, beforeAttack: attack => Owner.figure.TryWalk(attack.delta));
        }
        Game.Debug("Katar failed long attack: no target");
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
