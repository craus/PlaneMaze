using System;
using System.Collections;
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
            .Select(d => Owner.figure.Location.Shift(delta.Relative(d)).GetFigure<Unit>(u => u.Vulnerable))
            .Where(u => u != null);

        return await MultipleAttack(delta, targets);
    }

    public Attack GetAttack(Vector2Int delta, Unit target) => new Attack(
        delta, Owner.figure, target.figure, AttackLocation(delta, target), DefenceLocation(delta, target), damage
    );

    public virtual async Task BeforeAttack(Attack attack) { }

    public virtual async Task AfterAttack(Attack attack) { }
    public virtual async Task BeforeMultipleAttack(Vector2Int delta) { }

    public virtual async Task AfterMultipleAttack(Vector2Int delta) { }


    public virtual Cell AttackLocation(Vector2Int delta, Unit target) => Owner.figure.Location;
    public virtual Cell DefenceLocation(Vector2Int delta, Unit target) => target.figure.Location;

    public virtual async Task<bool> MultipleAttack(Vector2Int delta, IEnumerable<Unit> targets) {
        await BeforeMultipleAttack(delta);
        var result = (await Task.WhenAll(targets.Select(t => Attack(delta, t)))).Any(a => a);
        if (!result) return false;
        await AfterMultipleAttack(delta);
        return true;
    }

    public virtual async Task<bool> Attack(
        Vector2Int delta,
        Unit target
    ) {
        if (target == null) return false;
        return await Attack(delta, target, BeforeAttack, AfterAttack);
    }

    public async Task<bool> Attack(
        Vector2Int delta, 
        Unit target,
        Func<Attack, Task> beforeAttack = null, 
        Func<Attack, Task> afterAttack = null 
    ) {
        beforeAttack ??= BeforeAttack;
        afterAttack ??= AfterAttack;

        if (!Game.CanAttack(Owner, target, this, AttackLocation(delta, target), DefenceLocation(delta, target))) {
            Game.Debug($"Weapon {this}: cannot attack target");
            return false;
        }

        target.figure.Location.OnOccupyingUnitAttacked(target);

        var attack = GetAttack(delta, target);
        await beforeAttack(attack);
        if (!Owner.alive) {
            Game.Debug($"Weapon {this}: owner died before attack");
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
        await Owner.Attack(attack);
        await afterAttack(attack);

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
