using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Slime : Monster
{
    public int size = 1;

    public int cooldown;
    public int currentCooldown;

    public override void Awake() {
        base.Awake();
        UpdateSprite();
        damage += size;
        cooldown = 1 + size;
        currentCooldown = cooldown;
    }

    public async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            return await Attack(newPosition.GetFigure<Player>());
        }
        return false;
    }

    private void UpdateSprite() {
    }

    protected override async Task MakeMove() {
        --currentCooldown;
        if (currentCooldown > 0) {
            return;
        }

        Vector2Int playerDelta = Player.instance.figure.location.position - figure.location.position;

        if (playerDelta.SumDelta() == 1) {
            await TryAttack(playerDelta);
        } else {
            var delta = moves.Rnd();
            if (!await TryAttack(delta)) {
                if (!await SmartWalk(delta)) {
                    await figure.FakeMove(delta);
                }
            }
        }

        currentCooldown = cooldown;
    }
}
