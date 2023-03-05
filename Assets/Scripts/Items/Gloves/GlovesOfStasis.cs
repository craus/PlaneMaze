using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class GlovesOfStasis : MonoBehaviour, IAttackModifier
{
    public int freezeDuration = 2;
    public int invulnerabilityDuration = 2;

    public int Priority => 0;

    public async Task ModifyAttack(Attack attack) {
        attack.afterAttack.Add(
            async () => {
                await attack.to.GetComponent<MovesReserve>().Freeze(freezeDuration);
                await attack.to.GetComponent<Invulnerability>().Gain(invulnerabilityDuration);
            }
        );
    }
}
