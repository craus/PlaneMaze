using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Wolf : Monster
{
    protected override async Task MakeMove() {

        Vector2Int playerDelta = Player.instance.figure.Location.position - figure.Location.position;

        if (playerDelta.SumDelta() == 1) {
            if (await TryAttack(playerDelta)) {
                return;
            }
        }

        for (int i = 0; i < 2; i++) {
            var delta = Moves.Rnd();
            GetComponent<SpriteDirection>().SetDirection(delta);
            if (!await SmartWalk(delta)) {
                await figure.FakeMove(delta);
            }
        }
    }
}
