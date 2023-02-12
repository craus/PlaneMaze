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

    public override async Task Hit(int damage) {
        await base.Hit(damage);
        await Helpers.TeleportAway(figure, teleportRadius);
    }

    public void OnEnable() {
        GameEvents.instance.onUnitDeath.AddListener(OnUnitDeath);
    }

    public void OnDisable() {
        GameEvents.instance.onUnitDeath.RemoveListener(OnUnitDeath);
    }

    public void OnUnitDeath(Unit unit) {
        if ((unit.figure.location.position - figure.location.position).MaxDelta() <= deathDetectionRadius && unit is Monster) {
            figure.location.Vicinity(damageRadius)
                .Select(c => c.GetFigure<Unit>())
                .Where(u => u != null && u != this)
                .ForEach(async u => await u.Hit(deathDamage));
        }
    }
}
