using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DemonSpawn : Monster
{
    public Vector2Int currentDirection;
    public SpriteRenderer sprite;

    public Fireball fireballSample;

    public int range = 18;

    public override void Awake() {
        base.Awake();
        currentDirection = Vector2Int.zero;
        UpdateSprite();

        new ValueTracker<Vector2Int>(() => currentDirection, v => {
            currentDirection = v; UpdateSprite();
        });
    }

    private void UpdateSprite() {
    }

    private async Task LaunchFireball(Vector2Int direction) {
        var fireball = Game.GenerateFigure(figure.Location.Shift(direction), fireballSample);
        fireball.currentDirection = direction;
        fireball.UpdateSprite();
        await fireball.CheckExplode();
    }

    protected override async Task MakeMove() {
        Vector2Int playerDelta = Player.instance.figure.Location.position - figure.Location.position;
        if (playerDelta.x == 0 || playerDelta.y == 0) {
            playerDelta /= (int)(playerDelta.magnitude + .5f);

            if (Helpers.RayCast(figure.Location, playerDelta, target: c => c.GetFigure<Player>() != null, distance: range) != null) {
                await LaunchFireball(playerDelta);
                return;
            }
        }

        for (int i = 0; i < 1; i++) {
            if (Rand.rndEvent(0.5f)) {
                var delta = Helpers.Moves.Rnd();
                if (!await TryAttack(delta)) {
                    if (!await figure.TryWalk(delta)) {
                        await figure.FakeMove(delta);
                    }
                }
            }
        }
    }
}
