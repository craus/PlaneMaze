using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TemporalGhost : Monster
{
    [SerializeField] private int minSafeDistance = 16;
    [SerializeField] private int teleportBackDistance = 16;
    [SerializeField] private int teleportAfterAttackMaxDistance = 16;

    public override bool Flying => true;
    public override bool HasSoul => false;
    public override int Money => 0;
    public override bool FreeCell(Cell cell) => true;
    public override bool Vulnerable => false;

    public override void Awake() {
        base.Awake();
        Player.instance.figure.afterMove.Add(AfterPlayerMove);
    }

    private async Task AfterPlayerMove(Cell from, Cell to) {
        if (from == to) return;
        Vector2Int deltaWithPlayer = Player.instance.figure.Location.position - figure.Location.position;

        if (deltaWithPlayer.MaxDelta() >= minSafeDistance) return;

        Cell teleportDestination = figure.Location.Shift(
            Vector2Int.RoundToInt(-teleportBackDistance * ((Vector2)deltaWithPlayer).normalized)
        );

        await Helpers.Teleport(GetComponent<Figure>(), teleportDestination, false);
    }

    protected override async Task MakeMove() {
        var playerDelta = Helpers.StepAtDirection(Player.instance.figure.Location.position - figure.Location.position);

        if (await TryAttack(playerDelta)) {
            if (this == null) return;
            await Helpers.TeleportAway(Player.instance.figure, teleportAfterAttackMaxDistance);
        }

        if (await SmartWalk(playerDelta)) {
            return;
        }
    }
}
