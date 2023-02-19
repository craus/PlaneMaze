﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class BubbleArmor : MonoBehaviour, IReceiveAttackModifier
{
    public int Priority => 0;

    public void ModifyAttack(Attack attack) {
        if (attack.damage < 1) {
            return;
        }
        attack.damage = 0;
        Destroy(gameObject);
    }
}
