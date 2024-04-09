using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GiantMosquito : Monster
{
    public override bool Flying => true;

    public override async Task AfterAttack(Vector2Int delta) {
        await base.AfterAttack(delta);
        await GetComponent<Health>().Heal(1);
    }

    protected override async Task MakeMove() {
        var playerDelta = PlayerDelta;
        if (playerDelta.SumDelta() == 1) {
            await SmartWalk(-playerDelta);
            return;
        }
        if (playerDelta.MaxDelta() == 2 && playerDelta.MinDelta() == 0) {
            var step = playerDelta / 2;
            if (await SmartWalk(step)) {
                await TryAttack(step);
            }
            return;
        }
        await SmartWalk(Helpers.Moves.Rnd());
    }
}
