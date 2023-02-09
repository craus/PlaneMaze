using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Harpy : Monster
{
    public async Task Attack(Unit target) {
        var ap = Instantiate(attackProjectile);
        ap.gameObject.SetActive(true); // object was inactive for unknown reason
        ap.transform.position = target.transform.position;

        await Task.Delay(100);

        target.Hit(damage);

        if (ap != null) {
            Destroy(ap);
        }
    }

    public async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = figure.location.Shift(2*delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            await Attack(newPosition.GetFigure<Player>());
            return true;
        }
        return false;
    }

    public override async Task Move() {
        var delta = moves.Rnd();
        if (!await TryAttack(delta)) {
            if (!await figure.TryWalk(delta)) {
                await figure.FakeMove(delta);
            }
        }
    }
}
