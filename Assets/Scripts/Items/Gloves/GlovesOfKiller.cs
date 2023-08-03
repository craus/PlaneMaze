using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class GlovesOfKiller : MonoBehaviour, IAttackModifier
{
    public int Priority => 0;

    public async Task ModifyAttack(Attack attack) {
        attack.afterAttack.Add(
            async () => {
                if (!attack.to.GetComponent<Unit>().alive) {
                    await attack.from.GetComponent<Invulnerability>().Gain(4);
                    await attack.from.GetComponent<MovesReserve>().Haste(3);
                }
            }
        );
    }
}
