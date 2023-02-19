using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Axe : Weapon
{
    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);

    public List<Vector2Int> attackDirections = new List<Vector2Int>() {
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
    };

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var targets = attackDirections
            .Select(d => Owner.figure.location.Shift(delta.Relative(d)).GetFigure<Unit>(u => u.Vulnerable))
            .Where(u => u != null);

        return (await Task.WhenAll(targets.Select(t => Attack(t)))).Any(a => a);
    }
}
