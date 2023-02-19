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
        var result = await base.TryAttack(delta);
        if (result) {
            if (Owner.alive) {
                if (!await Owner.figure.TryWalk(delta)) {
                    await Owner.figure.FakeMove(delta);
                }
            }
        }
        return result;
    }
}
