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
        attack.afterAttack.Add(
            async () => {
                if (attack.from != null && attack.from.GetComponent<Unit>().alive) {
                    if (await attack.from.TryWalk((attack.from.location.position - attack.to.location.position).StepAtDirectionDiagonal())) {
                        if (GetComponent<Item>().Owner != null && GetComponent<Item>().Owner.alive) {
                            await GetComponent<Item>().Owner.GetComponent<MovesReserve>().Haste(1);
                        }
                    }
                }
            }
        );
    }
}
