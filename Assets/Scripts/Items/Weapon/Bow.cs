using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Bow : Weapon
{
    public int range = 4;

    public Vector2Int chargedDirection;

    public override bool CanAttackOnHill => true;

    public GameObject iconCharged;
    public GameObject iconUncharged;

    public override async Task<bool> Attack(Vector2Int delta, Unit target) {
        var transformDelta = target.transform.position - Owner.transform.position;

        SoundManager.instance.rangedAttack.Play();

        var ap = Instantiate(attackProjectileSample);
        ap.transform.rotation = Quaternion.LookRotation(Vector3.forward, transformDelta.normalized);
        ap.transform.position = Owner.transform.position + transformDelta.normalized * 0.5f;
        await ap.transform.Move(target.transform.position, 0.02f * transformDelta.magnitude);

        if (ap != null) {
            Destroy(ap);
        }
        var attack = GetAttack(delta, target);
        await Owner.Attack(attack);
        await AfterAttack(attack);

        return true;
    }

    public void Awake() {
        UpdateIcon();
    }

    private void UpdateIcon() {
        iconCharged.SetActive(chargedDirection != Vector2Int.zero);
        iconUncharged.SetActive(chargedDirection == Vector2Int.zero);
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var currentPosition = Owner.figure.location;
        var target = Helpers.RayCast(Owner.figure.location, delta, target: c => c.GetFigure<Monster>(m => m.Vulnerable) != null, distance: range);
        if (target != null) {
            var victim = target.GetFigure<Monster>(m => m.Vulnerable);
            if (!Game.CanAttack(Owner, victim, this)) {
                chargedDirection = Vector2Int.zero;
                UpdateIcon();
                return false;
            }
            if (chargedDirection == delta) {
                chargedDirection = Vector2Int.zero;
                UpdateIcon();
                return await Attack(delta, victim);
            } else {
                chargedDirection = delta;
                SoundManager.instance.bowCharge.Play();
                UpdateIcon();
                return true;
            }
        }
        chargedDirection = Vector2Int.zero;
        UpdateIcon();
        return false;
    }

    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);
}
