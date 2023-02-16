using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Ghost : Monster
{
    public override bool Flying => true;
    public override int Money => 0;
    public override bool FreeCell(Cell cell) => !cell.figures.Any(f => f.GetComponent<Unit>() != null && f.GetComponent<Unit>().OccupiesPlace);

    protected override async Task MakeMove() {
        var playerDelta = Player.instance.figure.location.position - figure.location.position;
        if (Mathf.Abs(playerDelta.x) > Mathf.Abs(playerDelta.y)) {
            playerDelta.y = 0;
        } else {
            playerDelta.x = 0;
        }
        playerDelta /= (int)playerDelta.magnitude;

        if (!await SmartWalk(playerDelta)) {
            if (!await TryAttack(playerDelta)) {
                await SmartFakeMove(playerDelta);
            }
        }
    }
}
