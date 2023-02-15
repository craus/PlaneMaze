using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Dagger : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = Owner.figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Unit>() != null)) {
            return await Attack(newPosition.GetFigure<Unit>());
        }
        return false;
    }

    public override Task<bool> AfterFailedWalk(Vector2Int delta) => TryAttack(delta);
}
