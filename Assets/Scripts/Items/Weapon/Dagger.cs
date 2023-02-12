using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Dagger : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (Owner.figure.location.GetFigure<PeaceTrap>() != null) {
            return false;
        }

        var newPosition = Owner.figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Unit>() != null)) {
            await Attack(newPosition.GetFigure<Unit>());
            Debug.LogFormat("Dagger attacked");
            return true;
        }
        Debug.LogFormat("Dagger use failed");
        return false;
    }

    public override Task<bool> AfterFailedWalk(Vector2Int delta) => TryAttack(delta);
}
