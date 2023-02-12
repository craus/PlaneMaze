using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Dart : Weapon
{
    public int range = 4;

    public override async Task Attack(Unit target) {
        var delta = target.transform.position - Owner.transform.position;

        var ap = Instantiate(attackProjectileSample);
        ap.transform.rotation = Quaternion.LookRotation(Vector3.forward, delta.normalized);
        ap.transform.position = Owner.transform.position + delta * 0.5f;
        await ap.transform.Move(target.transform.position, 0.02f * delta.magnitude);

        if (ap != null) {
            Destroy(ap);
        }
        await target.Hit(damage);
        GetComponent<Item>().Drop();
        await GetComponent<Figure>().Move(target.figure.location, isTeleport: true);
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        if (Owner.figure.location.GetFigure<PeaceTrap>() != null) {
            return false;
        }
        var currentPosition = Owner.figure.location;
        for (int i = 0; i < range; i++) {
            currentPosition = currentPosition.Shift(delta);
            var enemy = currentPosition.GetFigure<Monster>();
            if (enemy != null) {
                await Attack(enemy);
                return true;
            }
        }
        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
