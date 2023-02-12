using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BlackMage : Monster
{
    public int deathDetectionRadius = 4;
    public int teleportRadius = 8;
    public int damageRadius = 4;
    public int deathDamage = 1;

    public GameObject soulSample;

    public override async Task Hit(int damage) {
        await base.Hit(damage);
        await Helpers.TeleportAway(figure, teleportRadius);
    }

    public void OnEnable() {
        GameEvents.instance.onUnitDeath.Add(OnUnitDeath);
    }

    public void OnDisable() {
        GameEvents.instance.onUnitDeath.Remove(OnUnitDeath);
    }

    public async Task DealDeathDamage(Unit target) {
        var ap = Instantiate(attackProjectile);
        ap.transform.position = target.figure.location.transform.position;

        await Task.Delay(100);
        await target.Hit(deathDamage);

        if (ap != null) {
            Destroy(ap);
        }
    }

    public async Task ConsumeSoul(Unit unit) {

        var soul = Instantiate(soulSample);
        soul.transform.position = unit.figure.location.transform.position;
        await soul.transform.Move(figure.location.transform.position, 0.1f);

        if (soul != null) {
            Destroy(soul);
        }

        foreach (var u in figure.location.Vicinity(damageRadius)
            .Select(c => c.GetFigure<Unit>())
            .Where(u => u != null && u != this)
        ) {
            await DealDeathDamage(u);
        }
    }

    public async Task OnUnitDeath(Unit unit) {
        if ((unit.figure.location.position - figure.location.position).MaxDelta() <= deathDetectionRadius && unit is Monster) {
            await ConsumeSoul(unit);
        }
    }
}
