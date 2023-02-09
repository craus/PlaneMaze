using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Monster : Unit
{
    public GameObject attackProjectile;
    public int damage = 1;

    public override void Awake() {
        base.Awake();
    }

    private List<Vector2Int> moves = new List<Vector2Int>() {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
    };

    public async void Attack(Unit target) {
        Debug.LogFormat($"Attack {target}"); 
        if (attackProjectile.activeSelf != true) {
            Debug.LogError($"attackProjectile.activeSelf != true");
        }
        var ap = Instantiate(attackProjectile);
        ap.transform.position = target.transform.position;
        if (ap.activeSelf != true) {
            Debug.LogError($"ap.activeSelf != true");
        }

        await Task.Delay(10000);

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

    public async Task Move() {
        var delta = moves.Rnd();
        if (!(await figure.TryWalk(delta))) {
            if (!TryAttack(delta)) {
                await figure.FakeMove(delta);
            }
        }
    }

    public override void Die() {
        base.Die();
        Game.instance.monsters.Remove(this);
    }
}
