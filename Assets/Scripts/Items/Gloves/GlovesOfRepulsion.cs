using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class GlovesOfRepulsion : MonoBehaviour, IAttackModifier
{
    public int Priority => throw new NotImplementedException();

    public async Task ModifyAttack(Attack attack) {
        attack.afterAttack.Add(
            async () => await attack.to.TryWalk((attack.to.location.position - attack.from.location.position).StepAtDirectionDiagonal())
        );
    }
}
