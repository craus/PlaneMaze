using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public abstract class Monster : Unit
{
    protected async Task<bool> SmartWalk(Vector2Int delta) {
        if (!Flying && figure.location.Shift(delta).GetFigure<WolfTrap>() != null) {
            return false;
        }
        return await figure.TryWalk(delta);
    }

    protected async Task<bool> SmartFakeMove(Vector2Int delta) {
        //if (figure.location.Shift(delta).GetFigure<WolfTrap>() != null) {
        //    return false;
        //}
        return await figure.FakeMove(delta);
    }

    public GameObject attackProjectile;
    public int damage = 1;

    public override void Awake() {
        base.Awake();
    }

    public async Task<bool> Attack(Unit target) {
        if (!Game.CanAttack(this, target)) {
            return false;
        }

        var ap = Instantiate(attackProjectile);
        ap.gameObject.SetActive(true); // object was inactive for unknown reason
        ap.transform.position = target.transform.position;

        await Task.Delay(100);

        await target.Hit(damage);

        if (ap != null) {
            Destroy(ap);
        }

        return true;
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
        await GetComponent<Invulnerability>().Spend(1);
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

    protected override async Task AfterDie() {
        Player.instance.gems += Money;
        Game.instance.monsters.Remove(this);
    }
}
