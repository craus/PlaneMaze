using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class HellGate : Monster
{
    public override bool HasSoul => false;
    public override int Money => 0;
    public override bool FireImmune => true;
    public override bool Movable => false;

    public int cooldown = 4;
    public int currentCooldown;

    public int aggroRadius = 6;
    public int spawnRadius = 4;

    public GameObject charged;

    public List<Monster> monsterSamples;

    public override void Awake() {
        base.Awake();
        currentCooldown = cooldown;
        UpdateSprite();
        new ValueTracker<int>(() => currentCooldown, v => {
            currentCooldown = v;
            UpdateSprite();
        });
    }

    private void UpdateSprite() {
        charged.SetActive(currentCooldown <= 1);
    }

    private async Task Spawn() {
        var spawnLocation = figure.Location.Vicinity(spawnRadius).Where(c => c.Free).Rnd();
        if (spawnLocation == null) {
            return;
        }
        var monster = Game.GenerateFigure(spawnLocation, monsterSamples.rnd());
        monster.poor = true;
    }

    protected override async Task MakeMove() {
        --currentCooldown;
        UpdateSprite();

        var playerDelta = Player.instance.figure.Location.position - figure.Location.position;
        if (playerDelta.MaxDelta() > aggroRadius) {
            return;
        }

        if (currentCooldown > 0) {
            return;
        }

        await Spawn();

        currentCooldown = cooldown;
        UpdateSprite();
    }
}
