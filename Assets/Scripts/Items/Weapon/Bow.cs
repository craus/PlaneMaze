using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Bow : Weapon
{
    public int range = 4;

    public override bool CanAttackOnHill => true;

    public override async Task<bool> Attack(Unit target) {
        if (!Game.CanAttack(Owner, target, this)) {
            return false;
        }

        var delta = target.transform.position - Owner.transform.position;

        var ap = Instantiate(attackProjectileSample);
        ap.transform.rotation = Quaternion.LookRotation(Vector3.forward, delta.normalized);
        ap.transform.position = Owner.transform.position + delta * 0.5f;
        await ap.transform.Move(target.transform.position, 0.02f * delta.magnitude);

        if (ap != null) {
            Destroy(ap);
        }
        await target.Hit(damage);

        return true;
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var currentPosition = Owner.figure.location;
        var target = Helpers.RayCast(Owner.figure.location, delta, target: c => c.GetFigure<Monster>(m => m.Vulnerable) != null, distance: range);
        if (target != null) {
            if (await Attack(target.GetFigure<Monster>(m => m.Vulnerable))) {
                return true;
            }
        }
        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
