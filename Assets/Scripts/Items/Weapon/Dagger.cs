using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Dagger : Weapon
{
    public override Task<bool> AfterFailedWalk(Vector2Int delta) => TryAttack(delta);

    public override async Task AfterAttack(Attack attack) {
        if (this != null && attack.to != null) {
            await attack.to.GetComponent<Disarm>().Gain(1);
        }
        await base.AfterAttack(attack);
    }
}
