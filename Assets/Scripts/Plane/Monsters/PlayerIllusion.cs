using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerIllusion : Illusion
{
    protected override async Task MakeMove() {
        if (!await WalkOrFakeMove(-Player.instance.lastMove)) {
            await TryAttack(-Player.instance.lastMove);
        }
    }
}
