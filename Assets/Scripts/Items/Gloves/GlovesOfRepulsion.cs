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
    public int Priority => 0;

    public async Task ModifyAttack(Attack attack) {
        attack.afterAttack.Add(
            async () => {
                if (attack.to != null && attack.to.GetComponent<Unit>().alive && attack.to.GetComponent<Unit>().Movable) {
                    if (await attack.to.TryWalk((attack.to.Location.position - attack.from.Location.position).StepAtDirectionDiagonal())) {
                        if (attack.to != null && attack.to.GetComponent<Unit>().alive) {
                            await attack.to.GetComponent<MovesReserve>().Freeze(2);
                        }
                    }
                }
            }
        );
    }
}
