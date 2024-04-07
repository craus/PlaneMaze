using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Imp : Monster
{
    public Vector2Int currentDirection;

    public Transform spriteParent;
    public SpriteRenderer sprite;
    public SpriteRenderer chargedSprite;

    public bool charged = false;
    public bool woundedLastTurn = false;

    public override void Awake() {
        base.Awake();
        currentDirection = Helpers.Moves.Rnd();
        UpdateSprite();
        new ValueTracker<Vector2Int>(() => currentDirection, v => {
            currentDirection = v; UpdateSprite();
        });
        new ValueTracker<bool>(() => charged, v => {
            charged = v; UpdateSprite();
        });
        new ValueTracker<bool>(() => woundedLastTurn, v => woundedLastTurn = v);
    }

    public override Task Hit(Attack attack) {
        woundedLastTurn = true;
        return base.Hit(attack);
    }

    private void UpdateSprite() {
        if (this == null) {
            return;
        }
        if (currentDirection.x < 0) {
            sprite.flipX = false;
            chargedSprite.flipX = false;
        } else if (currentDirection.x > 0) {
            sprite.flipX = true;
            chargedSprite.flipX = true;
        } else {
            sprite.flipX = Rand.rndEvent(0.5f, Rand.visual);
            chargedSprite.flipX = Rand.rndEvent(0.5f, Rand.visual);
        }
        if (currentDirection.y > 0) {
            spriteParent.transform.rotation = Quaternion.Euler(0, 0, -35 * (sprite.flipX ? -1 : 1));
        } else if (currentDirection.y < 0) {
            spriteParent.transform.rotation = Quaternion.Euler(0, 0, 35 * (sprite.flipX ? -1 : 1));
        } else {
            spriteParent.transform.rotation = Quaternion.identity;
        }
        sprite.enabled = !charged;
        chargedSprite.enabled = charged;
    }

    public override async Task AfterAttack(Vector2Int delta) {
        await base.AfterAttack(delta);
        Game.GenerateFigure(figure.Location.Shift(delta), Library.instance.fire);
    }

    public async Task DealChargeDamage() {
        foreach (var u in figure.Location.Vicinity(1)
            .SelectMany(c => c.GetFigures<Unit>())
            .ToList()
        ) {
            await Attack(u);
        }
        foreach (var cell in figure.Location.Vicinity(1).Where(c => c.Free && c.figures.Count == 0)) {
            Game.GenerateFigure(cell, Library.instance.fire);
        }
    }

    protected override async Task MakeMove() {
        if (charged) {
            charged = false;
            await DealChargeDamage();
            return;
        }

        if (woundedLastTurn) {
            woundedLastTurn = false;
            charged = true;
            UpdateSprite();
            return;
        }

        if (!await SmartWalk(currentDirection)) {
            if (!await TryAttack(currentDirection)) {
                await SmartFakeMove(currentDirection);
                currentDirection = Helpers.Moves.Rnd(m => figure.Location.Shift(m).Free);
                UpdateSprite();
            }
        }
    }
}
