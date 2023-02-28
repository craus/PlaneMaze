using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Dart : Weapon
{
    public int range = 4;

    public override bool CanAttackOnHill => true;

    public override async Task<bool> Attack(Unit target) {
        if (!Game.CanAttack(Owner, target, this)) {
            return false;
        }

        var delta = target.transform.position - Owner.transform.position;

        SoundManager.instance.rangedAttack.Play();

        var ap = Instantiate(attackProjectileSample);
        ap.transform.rotation = Quaternion.LookRotation(Vector3.forward, delta.normalized);
        ap.transform.position = Owner.transform.position + delta * 0.5f;
        await ap.transform.Move(target.transform.position, 0.02f * delta.magnitude);

        if (ap != null) {
            Destroy(ap);
        }
        await DealDamage(target);
        GetComponent<Item>().Drop();
        await GetComponent<Figure>().Move(target.figure.location, isTeleport: true);

        return true;
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var currentPosition = Owner.figure.location;
        for (int i = 0; i < range; i++) {
            currentPosition = currentPosition.Shift(delta);
            var enemy = currentPosition.GetFigure<Monster>(m => m.Vulnerable);
            if (enemy != null) {
                if (await Attack(enemy)) {
                    return true;
                }
            }
            if (!currentPosition.Free) {
                return false;
            }
        }
        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
