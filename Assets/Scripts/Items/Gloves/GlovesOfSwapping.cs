using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class GlovesOfSwapping : MonoBehaviour, IAttackModifier
{
    public int Priority => 0;

    public async Task ModifyAttack(Attack attack) {
        attack.afterAttack.Add(
            async () => {
                var fromLocation = attack.from.location;
                var toLocation = attack.to.location;
                await Task.WhenAll(
                    attack.to.Move(fromLocation),
                    attack.from.Move(toLocation)
                );
            }
        );
    }
}
