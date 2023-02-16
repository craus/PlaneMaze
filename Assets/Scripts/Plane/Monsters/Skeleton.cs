using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Skeleton : Monster
{
    public SpriteRenderer sprite;

    public async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            return await Attack(newPosition.GetFigure<Player>());
        }
        return false;
    }

    protected override async Task MakeMove() {
        var playerDelta = Player.instance.figure.location.position - figure.location.position;
        if (playerDelta.MaxDelta() > 4) {
            return;
        }
        if (Mathf.Abs(playerDelta.x) > Mathf.Abs(playerDelta.y)) {
            playerDelta.y = 0;
        } else {
            playerDelta.x = 0;
        }
        playerDelta /= (int)playerDelta.magnitude;

        if (!await SmartWalk(playerDelta)) {
            if (!await TryAttack(playerDelta)) {
                await SmartFakeMove(playerDelta);
            }
        }
    }
}
