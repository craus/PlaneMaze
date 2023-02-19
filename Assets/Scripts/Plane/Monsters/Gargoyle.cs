using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Gargoyle : Monster
{
    public override bool Flying => true;

    public Vector2Int currentDirection;
    public SpriteRenderer sprite;
    public SpriteRenderer awakenSprite;

    public int range = 18;

    public override void Awake() {
        base.Awake();
        currentDirection = Vector2Int.zero;
        UpdateSprite();
    }

    private void UpdateSprite() {
        awakenSprite.gameObject.SetActive(currentDirection != Vector2Int.zero);
    }

    protected override async Task MakeMove() {
        if (currentDirection != Vector2Int.zero) {
            for (int i = 0; i < 2; i++) {
                if (!await SmartWalk(currentDirection)) {
                    if (!await TryAttack(currentDirection)) {
                        await SmartFakeMove(currentDirection);
                        currentDirection = Vector2Int.zero;
                        UpdateSprite();
                    }
                    break;
                }
            }
        } else {
            Vector2Int playerDelta = Player.instance.figure.location.position - figure.location.position;
            if (playerDelta.x != 0 && playerDelta.y != 0) {
                return;
            }
            playerDelta /= (int)(playerDelta.magnitude + .5f);

            if (Helpers.RayCast(figure.location, playerDelta, target: c => c.GetFigure<Player>() != null, distance: range) != null) {
                currentDirection = playerDelta;
                UpdateSprite();
            }
        }
    }
}
