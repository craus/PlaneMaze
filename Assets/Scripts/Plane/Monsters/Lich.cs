﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Lich : Monster
{
    public override bool HasSoul => false;
    public override bool Boss => true;

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

    public GameObject charged;

    public override bool Vulnerable => base.Vulnerable && (phylactery == null || !phylactery.alive);

    public override void Awake() {
        base.Awake();
        if (!GameManager.instance.metagame.HasAscention<FasterBoss>()) {
            cooldown = 3;
        }
        currentCooldown = cooldown;
        UpdateSprite();
        new ValueTracker<int>(() => currentCooldown, v => {
            currentCooldown = v;
            UpdateSprite();
        });
    }

    public override void OnGameStart() {
        base.OnGameStart();
        var phylacteryLocation = Game.instance.mainWorld.cells.Where(c => !c.Wall && c.figures.Count() == 0).Rnd();
        phylactery = Game.GenerateFigure(phylacteryLocation, phylacterySample);
        GetComponent<Invulnerability>().UpdateIcons();
    }

    private void UpdateSprite() {
        charged.SetActive(currentCooldown <= 1);
    }

    public override void PlayAttackSound() => SoundManager.instance.monsterRangedAttack.Play();

    public override async Task Hit(Attack attack) {
        await base.Hit(attack);
        if (alive) {
            attack.afterAttack.Add(async () => await Helpers.TeleportAway(figure, teleportRadius));
        }
    }

    private async Task TryWalkFromPlayer() {
        var playerDelta = Player.instance.figure.Location.position - figure.Location.position;

        if (!await SmartWalk(-playerDelta.StepAtDirection())) {
            await SmartFakeMove(-playerDelta.StepAtDirection());
        }
    }

    private async Task SpawnSkeleton() {
        var spawnLocation = figure.Location.Vicinity(spawnRadius).Where(c => c.Free).Rnd();
        if (spawnLocation == null) {
            return;
        }
        Game.GenerateFigure(spawnLocation, skeletonSample);
    }

    protected override async Task MakeMove() {
        --currentCooldown;
        UpdateSprite();

        var playerDelta = Player.instance.figure.Location.position - figure.Location.position;
        if (playerDelta.MaxDelta() > aggroRadius) {
            return;
        }

        if (currentCooldown > 0) {
            await TryWalkFromPlayer();
            return;
        }

        if (playerDelta.MinDelta() == 0 && minAttackRange <= playerDelta.MaxDelta() && playerDelta.MaxDelta() <= maxAttackRange) {
            if (await TryAttack(playerDelta)) {
                currentCooldown = cooldown;
                UpdateSprite();
                return;
            }
        }

        await SpawnSkeleton();

        currentCooldown = cooldown;
        UpdateSprite();
    }

    public override async Task Die() {
        await base.Die();
        var win = Game.instance.Win();
    }
}
