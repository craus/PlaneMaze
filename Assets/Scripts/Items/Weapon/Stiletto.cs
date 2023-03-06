using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Stiletto : Weapon
{
    public async override Task AfterAttack(Attack attack) {
        await base.AfterAttack(attack);
        if (Owner != null && Owner.alive) {
            await Owner.GetComponent<MovesReserve>().Haste(1);
        }
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (!await base.TryAttack(delta)) {
            return false;
        }
        if (Owner.alive) {
            if (!await Owner.figure.TryWalk(-delta)) {
                await Owner.figure.FakeMove(-delta);
            }
        }
        return true;
    }

    public override Task<bool> AfterFailedWalk(Vector2Int delta) => TryAttack(delta);
}
