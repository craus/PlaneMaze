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

    public async Task<bool> TryAttack(Vector2Int delta) {
        if (figure.location.GetFigure<PeaceTrap>() != null) {
            return false;
        }

        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            await Attack(newPosition.GetFigure<Player>());
            return true;
        }
        return false;
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
        if (!await figure.TryWalk(currentDirection)) {
            if (!await TryAttack(currentDirection)) {
                await figure.FakeMove(currentDirection);
                currentDirection *= -1;
                UpdateSprite();
            }
        }
    }
}
