using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Goblin : Monster
{
    public async void Attack(Unit target) {
        var ap = Instantiate(attackProjectile);
        ap.gameObject.SetActive(true); // object was inactive for unknown reason
        ap.transform.position = target.transform.position;

        await Task.Delay(100);

        target.Hit(damage);

        if (ap != null) {
            Destroy(ap);
        }
    }

    public bool TryAttack(Vector2Int delta) {
        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            Attack(newPosition.GetFigure<Player>());
            return true;
        }
        return false;
    }

    public override async Task Move() {
        var delta = moves.Rnd();
        if (!(await figure.TryWalk(delta))) {
            if (!TryAttack(delta)) {
                await figure.FakeMove(delta);
            }
        }
    }

    public override void Die() {
        base.Die();
        Player.instance.gems++;
        Game.instance.monsters.Remove(this);
    }
}
