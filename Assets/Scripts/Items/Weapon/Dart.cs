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

    public override async Task<bool> Attack(Vector2Int delta, Unit target, Cell targetCell) {
        if (!Game.CanAttack(Owner, target, this)) {
            return false;
        }

        var transformDelta = targetCell.transform.position - Owner.transform.position;
        var direction = Helpers.StepAtDirection(targetCell.position - Owner.figure.Location.position);

        SoundManager.instance.rangedAttack.Play();

        var ap = Instantiate(attackProjectileSample);
        ap.transform.rotation = Quaternion.LookRotation(Vector3.forward, transformDelta.normalized);
        ap.transform.position = Owner.transform.position + transformDelta.normalized * 0.5f;
        await ap.transform.Move(targetCell.transform.position, 0.02f * transformDelta.magnitude);

        await GetComponent<Item>().Drop();
        await GetComponent<Figure>().Move(targetCell.Shift(-direction), isTeleport: true);

        if (ap != null) {
            Destroy(ap);
        }
        var attack = GetAttack(delta, target);
        await Owner.Attack(attack);
        await AfterAttack(attack);

        return true;
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var currentPosition = Owner.figure.Location;
        for (int i = 0; i < range; i++) {
            currentPosition = currentPosition.Shift(delta);
            if (await Attack(delta, currentPosition)) {
                return true;
            }
            if (!currentPosition.Free) {
                return false;
            }
        }
        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
