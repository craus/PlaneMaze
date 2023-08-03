using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Stiletto : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (!await base.TryAttack(delta)) {
            return false;
        }
        if (Owner.alive) {
            if (!await Owner.figure.TryWalk(-delta)) {
                await Owner.figure.FakeMove(-delta);
            } else {
                if (Owner.alive) {
                    await Owner.GetComponent<MovesReserve>().Haste(1);
                }
            }
        }
        return true;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
