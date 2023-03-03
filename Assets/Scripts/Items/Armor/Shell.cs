using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Shell : MonoBehaviour, IReceiveAttackModifier
{
    public int Priority => 0;
    public int invulnerabilityDuration = 3;

    public async Task ModifyAttack(Attack attack) {
        if (attack.damage < 1) {
            return;
        }
        await GetComponent<Item>().Owner.GetComponent<Invulnerability>().Gain(invulnerabilityDuration);
    }
}
