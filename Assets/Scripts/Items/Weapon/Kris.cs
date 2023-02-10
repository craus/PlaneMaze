using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Kris : Weapon
{
    public void Awake() {
        attackProjectile.SetActive(false);
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (Owner.figure.location.GetFigure<PeaceTrap>() != null) {
            return false;
        }

        Debug.LogFormat($"Kris attacks from location {Owner.figure.location} at direction {delta}");

        var leftPosition = Owner.figure.location.Shift(delta.Relative(1, 1));
        Debug.LogFormat($"left position is {leftPosition}");
        if (leftPosition.figures.Any(f => f.GetComponent<Unit>() != null)) {
            await Attack(leftPosition.GetFigure<Unit>());
            return true;
        }

        var rightPosition = Owner.figure.location.Shift(delta.Relative(1, -1));
        Debug.LogFormat($"right position is {rightPosition}");
        if (rightPosition.figures.Any(f => f.GetComponent<Unit>() != null)) {
            await Attack(rightPosition.GetFigure<Unit>());
            return true;
        }

        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
