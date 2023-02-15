using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public abstract class Monster : Unit
{
    public GameObject attackProjectile;
    public int damage = 1;

    public override void Awake() {
        base.Awake();
    }

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

    protected List<Vector2Int> moves = new List<Vector2Int>() {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
    };

    protected virtual async Task MakeMove() {
    }

    public async Task Move() {
        if (!alive) {
            return;
        }
        if (GetComponent<MovesReserve>().Current < 0) {
            await GetComponent<MovesReserve>().Haste(1);
            return;
        }
        await MakeMove();
        if (!alive) {
            return;
        }
        for (int i = 0; i < 10 && GetComponent<MovesReserve>().Current > 0; i++) {
            await GetComponent<MovesReserve>().Freeze(1);
            await MakeMove(); 
            if (!alive) {
                return;
            }
        }
    }

    public override async Task Die() {
        await base.Die();
        Player.instance.gems++;
        Game.instance.monsters.Remove(this);
    }
}
