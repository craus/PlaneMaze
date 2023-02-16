using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Skeleton : Monster
{
    public override bool HasSoul => false;
    public override int Money => 0;

    public bool active = true;

    public int reviveCooldown = 30;
    public int currentReviveCooldown;

    public GameObject modelActive;
    public GameObject modelInactive;

    public override bool OccupiesPlace => active;

    public override bool Vulnerable => base.Vulnerable && active;

    public override void Awake() {
        base.Awake();
        UpdateSprite();
    }

    private async Task Revive() {
        active = true;
        await GetComponent<MovesReserve>().Freeze(1);
        UpdateSprite();
    }

    private void UpdateSprite() {
        modelActive.SetActive(active);
        modelInactive.SetActive(!active);
    }

    protected override async Task MakeMove() {
        if (!active) {
            --currentReviveCooldown;
            if (currentReviveCooldown <= 0 && figure.location.Free) {
                await Revive();
            }
            return;
        }
        var playerDelta = Player.instance.figure.location.position - figure.location.position;
        if (playerDelta.MaxDelta() > 4) {
            return;
        }
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

    public override async Task Die() {
        if (!active) {
            return;
        }
        await BeforeDie();
        active = false;
        currentReviveCooldown = reviveCooldown;
        UpdateSprite();
        foreach (var listener in GameEvents.instance.onUnitDeath.ToList()) {
            await listener(this);
        }
        await AfterDie();
    }

    protected override async Task AfterDie() {
        // do nothing, instead of base method
    }
}
