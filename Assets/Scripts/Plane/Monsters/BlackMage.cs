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

    public GameObject soulSample;
    public GameObject healSample;

    public List<GameObject> chargedModels;
    public List<GameObject> unchargedModels;

    public bool charged = false;
    public bool chargedAtLeastTurnAgo = false;

    public override void Awake() {
        base.Awake();
        UpdateIcon();

        new ValueTracker<bool>(() => charged, v => {
            charged = v;
            UpdateIcon();
        });
        new ValueTracker<bool>(() => chargedAtLeastTurnAgo, v => {
            chargedAtLeastTurnAgo = v;
            UpdateIcon();
        });
    }

    private void UpdateIcon() {
        chargedModels.ForEach(go => go.SetActive(chargedAtLeastTurnAgo));
        unchargedModels.ForEach(go => go.SetActive(!chargedAtLeastTurnAgo));
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

    public async Task ConsumeSoulAnimation(Vector3 soulPosition) {
        SoundManager.instance.consumeSoul.Play();

        var soul = Instantiate(soulSample, Game.instance.transform);
        soul.transform.position = soulPosition;
        await soul.transform.Move(figure.Location.transform.position, 0.1f);

        if (soul != null) {
            Destroy(soul);
        }
    }

    public async Task ConsumeOldSoul(Unit unit) {
        if (!unit.alive) {
            return;
        }
        if (unit.GetComponent<MovesReserve>().Current < 0) {
            return;
        }
        await unit.Die();

        await ConsumeSoulAnimation(unit.figure.Location.transform.position);

        await Heal();

        UpdateIcon();
    }

    public async Task Heal() {
        await Helpers.RunAnimation(Library.instance.healSample, transform);
        await GetComponent<Health>().Heal(1);
    }

    public async Task DealDeathDamage() {
        foreach (var u in figure.Location.Vicinity(damageRadius)
            .SelectMany(c => c.GetFigures<Unit>(u => u.SoulVulnerable))
            .ToList()
        ) {
            await DealDeathDamage(u);
        }
    }

    public async Task OnUnitDeath(Unit unit) {
        if (!alive) {
            return;
        }
        if (
            (unit.figure.Location.position - figure.Location.position).MaxDelta() <= deathDetectionRadius && 
            unit is Monster && 
            unit.HasSoul
        ) {
            unit.soul = false;
            charged = true;
            await ConsumeSoulAnimation(unit.figure.Location.transform.position);
            UpdateIcon();
        }
    }

    protected async override Task MakeMove() {
        if (chargedAtLeastTurnAgo) {
            charged = false;
            chargedAtLeastTurnAgo = false;
            UpdateIcon();
            await DealDeathDamage();
            if (charged) {
                chargedAtLeastTurnAgo = true;
                UpdateIcon();
                return;
            }
            return;
        }
        if (charged) {
            chargedAtLeastTurnAgo = true;
            UpdateIcon();
            return;
        }

        var closestGhost = figure.Location.Vicinity(deathDetectionRadius)
            .Select(c => c.GetFigure<Ghost>())
            .Where(g => g != null)
            .MinBy(g => (g.figure.Location.position - figure.Location.position).sqrMagnitude); // TODO: slow

        if (closestGhost == null) {
            return;
        }

        await ConsumeOldSoul(closestGhost);
    }
}
