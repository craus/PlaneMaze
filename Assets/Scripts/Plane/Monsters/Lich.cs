using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Lich : Monster
{
    public override bool HasSoul => false;

    public int teleportRadius = 8;

    public Skeleton skeletonSample;
    public Phylactery phylacterySample;
    public Phylactery phylactery;

    public int cooldown = 2;
    public int currentCooldown;

    public int aggroRadius = 6;
    public int spawnRadius = 4;
    public int minAttackRange = 3;
    public int maxAttackRange = 4;

    public override bool Vulnerable => base.Vulnerable && (phylactery == null || !phylactery.alive);

    public override void Awake() {
        base.Awake();
        currentCooldown = cooldown;
        UpdateSprite();
    }

    public override void OnGameStart() {
        base.OnGameStart();
        var phylacteryLocation = Game.instance.mainWorld.cells.Where(c => !c.Wall && c.figures.Count() == 0).Rnd();
        phylactery = Game.instance.GenerateFigure(phylacteryLocation, phylacterySample);
        Game.instance.monsters.Add(phylactery);
        GetComponent<Invulnerability>().UpdateIcons();
    }

    private void UpdateSprite() {
    }

    public override void PlayAttackSound() => SoundManager.instance.monsterRangedAttack.Play();

    public override async Task Hit(Attack attack) {
        await base.Hit(attack);
        if (alive) {
            await Helpers.TeleportAway(figure, teleportRadius);
        }
    }

    private async Task TryWalkFromPlayer() {
        var playerDelta = Player.instance.figure.location.position - figure.location.position;

        if (!await SmartWalk(-playerDelta.StepAtDirection())) {
            await SmartFakeMove(-playerDelta.StepAtDirection());
        }
    }

    private async Task SpawnSkeleton() {
        var spawnLocation = figure.location.Vicinity(spawnRadius).Where(c => c.Free).Rnd();
        if (spawnLocation == null) {
            return;
        }
        var child = Game.instance.GenerateFigure(spawnLocation, skeletonSample);
        Game.instance.monsters.Add(child);
    }

    protected override async Task MakeMove() {
        --currentCooldown;

        var playerDelta = Player.instance.figure.location.position - figure.location.position;
        if (playerDelta.MaxDelta() > aggroRadius) {
            return;
        }

        if (currentCooldown > 0) {
            await TryWalkFromPlayer();
            return;
        }

        if (playerDelta.MinDelta() == 0 && minAttackRange <= playerDelta.MaxDelta() && playerDelta.MaxDelta() <= maxAttackRange) {
            if (await TryAttack(playerDelta)) {
                return;
            }
        }

        await SpawnSkeleton();

        currentCooldown = cooldown;
    }

    public override async Task Die() {
        await base.Die();
        await Game.instance.Win();
    }
}
