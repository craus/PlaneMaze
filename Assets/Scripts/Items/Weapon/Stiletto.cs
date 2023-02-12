using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Stiletto : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (Owner.figure.location.GetFigure<PeaceTrap>() != null) {
            return false;
        }

        var newPosition = Owner.figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Unit>() != null)) {
            await Attack(newPosition.GetFigure<Unit>());
            if (!await Owner.figure.TryWalk(-delta)) {
                await Owner.figure.FakeMove(-delta);
            }
            return true;
        } 
        return false;
    }

    public override Task<bool> AfterFailedWalk(Vector2Int delta) => TryAttack(delta);
}
