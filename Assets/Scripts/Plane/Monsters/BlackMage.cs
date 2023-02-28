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

    public override async Task Hit(Attack attack) {
        await base.Hit(attack);
        if (alive) {
            await Helpers.TeleportAway(figure, teleportRadius);
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
        if (!unit.HasSoul) {
            return;
        }

        SoundManager.instance.consumeSoul.Play();

        var soul = Instantiate(soulSample);
        soul.transform.position = unit.figure.location.transform.position;
        await soul.transform.Move(figure.location.transform.position, 0.1f);

        if (soul != null) {
            Destroy(soul);
        }

        var heal = Instantiate(healSample);
        heal.transform.position = transform.position;
        await Helpers.Delay(0.1f);
        Destroy(heal);
        await GetComponent<Health>().Heal(1);

        foreach (var u in figure.location.Vicinity(damageRadius).ToList()
            .Select(c => c.GetFigure<Unit>())
            .Where(u => u != null && u != this)
        ) {
            await DealDeathDamage(u);
        }
    }

    public async Task OnUnitDeath(Unit unit) {
        if (!alive) {
            return;
        }
        if ((unit.figure.location.position - figure.location.position).MaxDelta() <= deathDetectionRadius && unit is Monster) {
            await ConsumeSoul(unit);
        }
    }
}
