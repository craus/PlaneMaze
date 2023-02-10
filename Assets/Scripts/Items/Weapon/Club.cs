using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Club : Weapon
{
    public void Awake() {
        attackProjectile.SetActive(false);
    }

    public async Task Attack(Unit target) {
        var startPosition = Vector3.Lerp(Owner.transform.position, target.transform.position, 0.25f);
        var endPosition = Vector3.Lerp(Owner.transform.position, target.transform.position, 0.75f);

        attackProjectile.SetActive(true);
        attackProjectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - Owner.transform.position);
        attackProjectile.transform.position = endPosition;

        await Task.Delay(100);
        target.Hit(damage);

        attackProjectile.SetActive(false);
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (Owner.figure.location.GetFigure<PeaceTrap>() != null) {
            return false;
        }

        var newPosition = Owner.figure.location.Shift(delta);
        var target = newPosition.GetFigure<Unit>();
        if (target != null) {
            if (target.figure.location.Shift(delta).Free) {
                await Attack(target);
                await target.figure.TryWalk(delta);
                return true;
            } else {
                await target.figure.FakeMove(delta);
                return true;
            }
        } else {
            return false;
        }
    }
}
