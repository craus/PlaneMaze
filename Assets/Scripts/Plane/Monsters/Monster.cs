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

    protected List<Vector2Int> moves = new List<Vector2Int>() {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
    };

    public virtual async Task Move() {
    }

    public override void Die() {
        base.Die();
        Player.instance.gems++;
        Game.instance.monsters.Remove(this);
    }
}
