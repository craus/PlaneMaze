using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Imp : Monster
{
    public Vector2Int currentDirection;
    public SpriteRenderer sprite;

    public override void Awake() {
        base.Awake();
        currentDirection = Helpers.Moves.Rnd();
        UpdateSprite();
        new ValueTracker<Vector2Int>(() => currentDirection, v => {
            currentDirection = v; UpdateSprite();
        });
    }

    private void UpdateSprite() {
        if (this == null) {
            return;
        }
        if (currentDirection.x < 0) {
            sprite.flipX = false;
        } else if (currentDirection.x > 0) {
            sprite.flipX = true;
        } else {
            sprite.flipX = Rand.rndEvent(0.5f, Rand.visual);
        }
        if (currentDirection.y > 0) {
            sprite.transform.rotation = Quaternion.Euler(0, 0, -35 * (sprite.flipX ? -1 : 1));
        } else if (currentDirection.y < 0) {
            sprite.transform.rotation = Quaternion.Euler(0, 0, 35 * (sprite.flipX ? -1 : 1));
        } else {
            sprite.transform.rotation = Quaternion.identity;
        }
    }

    public override async Task AfterAttack(Vector2Int delta) {
        await base.AfterAttack(delta);
        Game.GenerateFigure(figure.Location.Shift(delta), Library.instance.fire);
    }

    protected override async Task MakeMove() {
        if (!await SmartWalk(currentDirection)) {
            if (!await TryAttack(currentDirection)) {
                await SmartFakeMove(currentDirection);
                currentDirection = Helpers.Moves.Rnd(m => figure.Location.Shift(m).Free);
                UpdateSprite();
            }
        }
    }
}
