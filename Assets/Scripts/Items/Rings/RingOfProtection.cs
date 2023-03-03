using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class RingOfProtection : MonoBehaviour, IReceiveAttackModifier
{
    public int invulnerabilityDuration = 4;

    public int Priority => 1;

    public async Task ModifyAttack(Attack attack) {
        if (attack.damage < 1) {
            return;
        }
        attack.damage = 0;
        await GetComponent<Item>().Owner.GetComponent<Invulnerability>().Gain(invulnerabilityDuration);
        Destroy(gameObject);
    }
}
