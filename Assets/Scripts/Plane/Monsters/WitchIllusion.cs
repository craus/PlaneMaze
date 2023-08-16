using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class WitchIllusion : Illusion, IInvisibilitySource
{
    public bool Invisible => (Player.instance.figure.Location.position - GetComponent<Figure>().Location.position).MaxDelta() > 2;

    protected override async Task MakeMove() {
        if (PlayerDelta.MaxDelta() <= 2) {
            await SmartWalkOrFakeMove(Helpers.StepAtDirection(-PlayerDelta));
        } else {
            var selectedMove = Moves.Rnd();
            if (!await SmartWalkOrFakeMove(selectedMove)) {
                await TryAttack(selectedMove);
            }
        }
    }
}
