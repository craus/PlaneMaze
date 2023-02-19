using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class BackArmor : MonoBehaviour, IReceiveAttackModifier
{
    public int Priority => 0;

    public void ModifyAttack(Attack attack) {
        if (GetComponent<Item>().Owner.lastMove.Codirected(attack.to.position - attack.from.position)) {
            attack.damage -= 1;
        }
    }
}
