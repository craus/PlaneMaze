using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class GlovesOfRecoil : MonoBehaviour, IAttackModifier
{
    public int Priority => 0;

    public async Task ModifyAttack(Attack attack) {
        await attack.from.TryWalk((attack.from.location.position - attack.to.location.position).StepAtDirectionDiagonal());
    }
}
