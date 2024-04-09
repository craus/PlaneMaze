using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class PoisonousLake : Terrain
{
    public virtual bool CanAffect(Unit unit) => !unit.PoisonImmune && unit.Vulnerable && !unit.Flying;
    public override bool Scaring(Unit unit) => CanAffect(unit);

    public override void Awake() {
        base.Awake();
        GetComponent<Figure>().collide = async (from, figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null && CanAffect(victim)) {
                if (victim.GetComponent<Player>()) {
                    SoundManager.instance.swampDebuff.Play();
                }
                await victim.GetComponent<MovesReserve>().Freeze(1);
                if (!victim.GetComponent<Poison>().Active) {
                    await victim.GetComponent<Poison>().Gain(16);
                }
                GetComponent<Masked>().Trigger();
            }
        };
    }
}
