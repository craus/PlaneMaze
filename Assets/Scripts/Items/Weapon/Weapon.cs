using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Weapon : MonoBehaviour
{
    public int damage = 1;

    public virtual bool CanAttackOnHill => false;

    public Unit Owner => Player.instance;

    public GameObject attackProjectileSample;

    public virtual IEnumerable<Vector2Int> AttackVectors()
    {
        yield return new Vector2Int(1, 0);
    }

    public virtual async Task<bool> TryAttack(Vector2Int delta) {
        var targets = AttackVectors()
            .Select(d => Owner.figure.location.Shift(delta.Relative(d)).GetFigure<Unit>(u => u.Vulnerable))
            .Where(u => u != null);

        return (await Task.WhenAll(targets.Select(t => Attack(t)))).Any(a => a);
    }

    public async Task DealDamage(Unit target) {
        var currentDamage = damage;
        if (Inventory.instance.GetItem<RingOfStrength>()) {
            currentDamage++;
        }
        await target.Hit(currentDamage);
    }

    public virtual async Task<bool> Attack(Unit target) {
        if (!Game.CanAttack(Owner, target, this)) {
            return false;
        }

        var ap = Instantiate(attackProjectileSample);
        ap.transform.rotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - Owner.transform.position);
        ap.transform.position = Vector3.Lerp(Owner.transform.position, target.transform.position, 0.75f);

        await Task.Delay(100);
        if (ap != null) {
            Destroy(ap);
        }
        await DealDamage(target);

        return true;
    }

    public async virtual Task<bool> BeforeWalk(Vector2Int delta) {
        return false;
    }

    public async virtual Task<bool> AfterFailedWalk(Vector2Int delta) {
        return false;
    }
}
