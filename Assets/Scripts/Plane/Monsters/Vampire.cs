using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Vampire : Monster
{
    public bool batForm = true;
    public SpriteRenderer batSprite;
    public SpriteRenderer vampireSprite;

    public override bool Flying => batForm;
    public override bool HasSoul => false;

    public override void Awake() {
        base.Awake();
        ChangeForm(bat: true);
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (!batForm) {
            if (await base.TryAttack(delta)) {
                if (this != null) {
                    await GetComponent<Health>().Heal(1);
                }
                return true;
            }
            return false;
        }
        var leftPosition = figure.location.Shift(delta.Relative(1, 1) / 2);
        if (leftPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            if (await Attack(leftPosition.GetFigure<Unit>())) {
                return true;
            }
        }

        var rightPosition = figure.location.Shift(delta.Relative(1, -1) / 2);
        if (rightPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            if (await Attack(rightPosition.GetFigure<Unit>())) {
                return true;
            }
        }

        return false;
    }

    protected List<Vector2Int> BatMoves => new List<Vector2Int>() {
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
    };

    protected override List<Vector2Int> Moves => batForm ? BatMoves : base.Moves;

    private void ChangeForm(bool bat) {
        batForm = bat;
        batSprite.enabled = batForm;
        vampireSprite.enabled = !batForm;
    }

    protected override async Task MakeMove() {
        var playerDelta = Player.instance.figure.location.position - figure.location.position;
        if (playerDelta.MaxDelta() > 4 && !batForm) {
            ChangeForm(bat: true);
            return;
        }
        if (playerDelta.MaxDelta() <= 1 && batForm) {
            ChangeForm(bat: false);
            return;
        }
        if (batForm) {
            for (int i = 0; i < 1; i++) {
                var delta = Moves.Rnd();
                if (!await TryAttack(delta)) {
                    if (!await figure.TryWalk(delta)) {
                        await figure.FakeMove(delta);
                    }
                }
            }
        } else {
            playerDelta = Helpers.StepAtDirection(playerDelta);

            if (!await SmartWalk(playerDelta)) {
                if (!await TryAttack(playerDelta)) {
                    await SmartFakeMove(playerDelta);
                }
            }
        }
    }
}
