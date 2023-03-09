using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BlackMage : Monster
{
    public override bool HasSoul => false;

    public int deathDetectionRadius = 4;
    public int teleportRadius = 8;
    public int damageRadius = 4;
    public int deathDamage = 1;

    public Figure ghostSample;
    public GameObject soulSample;
    public GameObject healSample;

    public List<GameObject> chargedModels;
    public List<GameObject> unchargedModels;

    public bool charged = false;

    public override void Awake() {
        base.Awake();
        UpdateIcon();
    }

    private void UpdateIcon() {
        chargedModels.ForEach(go => go.SetActive(charged));
        unchargedModels.ForEach(go => go.SetActive(!charged));
    }

    public override async Task Hit(Attack attack) {
        await base.Hit(attack);
        if (alive) {
            attack.afterAttack.Add(async () => await Helpers.TeleportAway(figure, teleportRadius));
        }
    }

    public void OnEnable() {
        GameEvents.instance.onUnitDeath.Add(OnUnitDeath);
    }

    public void OnDisable() {
        GameEvents.instance.onUnitDeath.Remove(OnUnitDeath);
    }

    public override void PlayAttackSound() => SoundManager.instance.monsterRangedAttack.Play();

    public async Task DealDeathDamage(Unit target) {
        await Attack(target);
    }

    public async Task ConsumeSoul(Unit unit) {
        if (!unit.alive) {
            return;
        }
        if (unit.GetComponent<MovesReserve>().Current < 0) {
            return;
        }
        await unit.Die();

        SoundManager.instance.consumeSoul.Play();

        var soul = Instantiate(soulSample, Game.instance.transform);
        soul.transform.position = unit.figure.location.transform.position;
        await soul.transform.Move(figure.location.transform.position, 0.1f);

        if (soul != null) {
            Destroy(soul);
        }

        var heal = Instantiate(healSample, Game.instance.transform);
        heal.transform.position = transform.position;
        await Helpers.Delay(0.1f);
        Destroy(heal);
        await GetComponent<Health>().Heal(1);

        charged = true;
        UpdateIcon();
    }

    public async Task DealDeathDamage() {
        foreach (var u in figure.location.Vicinity(damageRadius)
            .Select(c => c.GetFigure<Unit>())
            .Where(u => u != null && u.SoulVulnerable)
            .ToList()
        ) {
            await DealDeathDamage(u);
        }
    }

    public async Task OnUnitDeath(Unit unit) {
        if (!alive) {
            return;
        }
        if ((unit.figure.location.position - figure.location.position).MaxDelta() <= deathDetectionRadius && unit is Monster && unit.HasSoul) {
            unit.soul = false;
            await SpawnGhost(unit.figure.location);
        }
    }

    protected async override Task MakeMove() {
        if (charged) {
            charged = false;
            UpdateIcon();
            await DealDeathDamage();
            return;
        }

        var closestGhost = figure.location.Vicinity(deathDetectionRadius)
            .Select(c => c.GetFigure<Ghost>())
            .Where(g => g != null)
            .MinBy(g => (g.figure.location.position - figure.location.position).sqrMagnitude);

        if (closestGhost == null) {
            return;
        }

        await ConsumeSoul(closestGhost);
    }

    private async Task SpawnGhost(Cell location) {
        var ghost = Game.instance.GenerateFigure(location, ghostSample);
        await ghost.GetComponent<MovesReserve>().Freeze(1);
    }
}
