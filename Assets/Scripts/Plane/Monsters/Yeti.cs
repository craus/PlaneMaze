using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Yeti : Monster
{
    public Vector2Int currentDirection;
    public SpriteRenderer sprite;

    public SpriteRenderer upSprite;
    public SpriteRenderer downSprite;

    public override void Awake() {
        base.Awake();
        currentDirection = Moves.Rnd();
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
            upSprite.flipX = false;
            downSprite.flipX = false;
        } else if (currentDirection.x > 0) {
            sprite.flipX = true;
            upSprite.flipX = true;
            downSprite.flipX = true;
        } else {
            sprite.flipX = upSprite.flipX = downSprite.flipX = Rand.rndEvent(0.5f);
        }
        upSprite.enabled = currentDirection.y > 0;
        downSprite.enabled = currentDirection.y < 0;
        sprite.enabled = currentDirection.y == 0;
    }

    protected override async Task MakeMove() {
        if (!await SmartWalk(currentDirection)) {
            if (!await TryAttack(currentDirection)) {
                await SmartFakeMove(currentDirection);
                currentDirection *= -1;
                UpdateSprite();
            }
        }
    }
}
