using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Flail : Weapon
{
    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);

    public override IEnumerable<Vector2Int> AttackVectors() => new List<Vector2Int>() {
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
    };

    public override async Task<bool> Attack(Vector2Int delta, Unit target) {
        if (!await base.Attack(delta, target)) {
            return false;
        }
        if (target.alive) {
            await target.GetComponent<MovesReserve>().Freeze(1);
        }
        return true;
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (await base.TryAttack(delta)) {
            if (Owner.alive) {
                await Owner.GetComponent<MovesReserve>().Freeze(1);
            }
            return true;
        } else {
            return false;
        }
    }
}
