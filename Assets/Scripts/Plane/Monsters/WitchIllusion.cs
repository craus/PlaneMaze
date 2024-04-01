using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class WitchIllusion : Illusion, IInvisibilitySource
{
    public bool Invisible => (Player.instance.figure.Location.position - GetComponent<Figure>().Location.position).MaxDelta() > 2;

    public event Action OnChange;

    public override void Awake() {
        base.Awake();

        Player.instance.figure.afterMove.Add(AfterPlayerMove);
        figure.afterMove.Add(AfterPlayerMove);
    }

    private async Task AfterPlayerMove(Cell from, Cell to) {
        OnChange();
    }

    protected override async Task MakeMove() {
        if (PlayerDelta.MaxDelta() <= 2) {
            await SmartWalkOrFakeMove(Helpers.StepAtDirection(-PlayerDelta));
        } else {
            var selectedMove = Helpers.Moves.Rnd();
            if (!await SmartWalkOrFakeMove(selectedMove)) {
                await TryAttack(selectedMove);
            }
        }
    }
}
