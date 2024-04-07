using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Succubus : Monster
{
    public override bool FireImmune => true;

    public SpriteRenderer sprite;

    protected override async Task MakeMove() {
        Vector2Int playerDelta = Player.instance.figure.Location.position - figure.Location.position;
        if (playerDelta.MaxDelta() <= 4) {
            if (playerDelta.SumDelta() <= 1) {
                await TryAttack(playerDelta);
                return;
            }
            playerDelta = Helpers.StepAtDirection(playerDelta);
            await Player.instance.figure.TryWalk(-playerDelta);
            return;
        }

        for (int i = 0; i < 1; i++) {
            if (Rand.rndEvent(0.5f)) {
                var delta = Helpers.Moves.Rnd();
                if (!await TryAttack(delta)) {
                    if (!await SmartWalk(delta)) {
                        await figure.FakeMove(delta);
                    }
                }
            }
        }
    }
}
