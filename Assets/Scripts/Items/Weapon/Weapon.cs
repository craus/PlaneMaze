﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Weapon : MonoBehaviour, IBeforeWalk, IAfterFailedWalk
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

        return (await Task.WhenAll(targets.Select(t => Attack(delta, t)))).Any(a => a);
    }

    public async Task DealDamage(Unit target) {
        var currentDamage = damage;
        if (Inventory.instance.GetItem<RingOfStrength>()) {
            currentDamage++;
        }
        await target.Hit(new Attack(Owner.figure, target.figure, currentDamage));
    }

    public virtual async Task BeforeAttack(Vector2Int delta) { }
    public virtual async Task AfterAttack(Vector2Int delta) { }

    public virtual Cell AttackLocation(Vector2Int delta, Unit target) => Owner.figure.location;
    public virtual Cell DefenceLocation(Vector2Int delta, Unit target) => target.figure.location;

    public virtual async Task<bool> Attack(Vector2Int delta, Unit target) {
        if (!Game.CanAttack(Owner, target, this, AttackLocation(delta, target), DefenceLocation(delta, target))) {
            return false;
        }

        await BeforeAttack(delta);
        if (!Owner.alive) {
            return true;
        }

        SoundManager.instance.meleeAttack.Play();

        var ap = Instantiate(attackProjectileSample);
        ap.transform.rotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - Owner.transform.position);
        ap.transform.position = Vector3.Lerp(Owner.transform.position, target.transform.position, 0.75f);

        await Helpers.Delay(0.1f);
        if (ap != null) {
            Destroy(ap);
        }
        await DealDamage(target);

        await AfterAttack(delta);

        return true;
    }

    public async virtual Task<bool> BeforeWalk(Vector2Int delta) {
        return false;
    }

    public async virtual Task<bool> BeforeWalk(Vector2Int delta, int priority) {
        if (priority == 1) {
            return await BeforeWalk(delta);
        }
        return false;
    }

    public async virtual Task<bool> AfterFailedWalk(Vector2Int delta, int priority) {
        if (priority == 1) {
            return await AfterFailedWalk(delta);
        }
        return false;
    }

    public async virtual Task<bool> AfterFailedWalk(Vector2Int delta) {
        return false;
    }
}
