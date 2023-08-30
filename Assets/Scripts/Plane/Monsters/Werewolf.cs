using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Werewolf : Monster
{
    public bool wolfForm = true;
    public SpriteRenderer wolfSprite;
    public SpriteRenderer werewolfSprite;

    public bool woundedLastTurn = false;

    public override void Awake() {
        base.Awake();
        ChangeForm(wolf: true);

        new ValueTracker<bool>(() => wolfForm, ChangeForm);
        new ValueTracker<bool>(() => woundedLastTurn, v => woundedLastTurn = v);
    }

    private void ChangeForm(bool wolf) {
        wolfForm = wolf;
        wolfSprite.enabled = wolfForm;
        werewolfSprite.enabled = !wolfForm;
    }

    public override Task Hit(Attack attack) {
        woundedLastTurn = true;
        return base.Hit(attack);
    }

    protected override async Task MakeMove() {
        var playerDelta = Player.instance.figure.Location.position - figure.Location.position;

        if (wolfForm) {
            if (playerDelta.SumDelta() <= 1 || woundedLastTurn) {
                woundedLastTurn = false;
                ChangeForm(wolf: false);
                return;
            }
            for (int i = 0; i < 2; i++) {
                var delta = Helpers.Moves.Rnd();
                GetComponent<SpriteDirection>().SetDirection(delta);
                if (!await SmartWalk(delta)) {
                    await figure.FakeMove(delta);
                }
            }
        } else {
            if (playerDelta.MaxDelta() > 4) {
                ChangeForm(wolf: true);
                return;
            }

            var toPlayer = Helpers.StepAtDirection(playerDelta);

            if (playerDelta.SumDelta() <= 1) {
                if (!await TryAttack(toPlayer)) {
                    await SmartFakeMove(toPlayer);
                }
                ChangeForm(wolf: true);
                await SmartWalk(-toPlayer);
                return;
            } 

            if (!await SmartWalk(toPlayer)) {
                await SmartFakeMove(toPlayer);
            }
        }
    }
}
