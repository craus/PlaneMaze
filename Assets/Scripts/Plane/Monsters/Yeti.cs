using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Yeti : Monster
{
    public Vector2Int currentDirection;
    public SpriteRenderer sprite;

    public override void Awake() {
        base.Awake();
        currentDirection = moves.Rnd();
        UpdateSprite();
    }

    private void UpdateSprite() {
        if (currentDirection.x < 0) {
            sprite.flipX = false;
        } else if (currentDirection.x > 0) {
            sprite.flipX = true;
        } else {
            sprite.flipX = Rand.rndEvent(0.5f);
        }
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
