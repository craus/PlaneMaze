using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Bow : Weapon
{
    public int range = 4;

    public bool charged = false;

    public override bool CanAttackOnHill => true;

    public GameObject iconCharged;
    public GameObject iconUncharged;

    public override async Task<bool> Attack(Unit target) {
        var delta = target.transform.position - Owner.transform.position;

        var ap = Instantiate(attackProjectileSample);
        ap.transform.rotation = Quaternion.LookRotation(Vector3.forward, delta.normalized);
        ap.transform.position = Owner.transform.position + delta * 0.5f;
        await ap.transform.Move(target.transform.position, 0.02f * delta.magnitude);

        if (ap != null) {
            Destroy(ap);
        }
        await DealDamage(target);

        return true;
    }

    public void Awake() {
        UpdateIcon();
    }

    private void UpdateIcon() {
        iconCharged.SetActive(charged);
        iconUncharged.SetActive(!charged);
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var currentPosition = Owner.figure.location;
        var target = Helpers.RayCast(Owner.figure.location, delta, target: c => c.GetFigure<Monster>(m => m.Vulnerable) != null, distance: range);
        if (target != null) {
            var victim = target.GetFigure<Monster>(m => m.Vulnerable);
            if (!Game.CanAttack(Owner, victim, this)) {
                charged = false;
                UpdateIcon();
                return false;
            }
            if (!charged) {
                charged = true;
                UpdateIcon();
                return true;
            } else {
                charged = false;
                UpdateIcon();
                return await Attack(victim);
            }
        }
        charged = false;
        UpdateIcon();
        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
