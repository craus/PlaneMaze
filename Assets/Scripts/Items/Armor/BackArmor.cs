using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class BackArmor : MonoBehaviour, IReceiveAttackModifier, ISideDefence
{
    public int Priority => 0;

    public bool GivesDefenceFrom(Vector2Int direction) => GetComponent<Item>().Owner.lastMove.Codirected(direction);

    public void Awake() {
        //GetComponent<Item>().onPick.Add(OnPick);
        //GetComponent<Item>().onDrop.Add(OnDrop);
    }

    public async Task ModifyAttack(Attack attack) {
        if (attack.attackLocation == null) {
            return;
        }
        if (GetComponent<Item>().Owner.lastMove.Codirected(attack.defenceLocation.position - attack.attackLocation.position)) {
            attack.damage -= 1;
        }
    }
}
