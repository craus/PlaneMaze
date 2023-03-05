using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class RingOfStrength : MonoBehaviour, IAttackModifier
{
    public int Priority => 0;

    public async Task ModifyAttack(Attack attack) {
        attack.damage++;
    }
}
