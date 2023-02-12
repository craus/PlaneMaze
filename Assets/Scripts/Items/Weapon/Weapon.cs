using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Weapon : MonoBehaviour
{
    public int damage = 1;

    public Unit Owner => Player.instance;

    public GameObject attackProjectile;

    public virtual async Task Attack(Unit target) {
        var startPosition = Vector3.Lerp(Owner.transform.position, target.transform.position, 0.25f);
        var endPosition = Vector3.Lerp(Owner.transform.position, target.transform.position, 0.75f);

        attackProjectile.SetActive(true);
        attackProjectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - Owner.transform.position);
        attackProjectile.transform.position = endPosition;

        await Task.Delay(100);
        attackProjectile.SetActive(false);
        await target.Hit(damage);
    }

    public async virtual Task<bool> BeforeWalk(Vector2Int delta) {
        return false;
    }

    public async virtual Task<bool> AfterFailedWalk(Vector2Int delta) {
        return false;
    }

    public async virtual Task<bool> TryAttack(Vector2Int delta) {
        return false;
    }
}
