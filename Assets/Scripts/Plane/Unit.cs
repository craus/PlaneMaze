using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Unit : MonoBehaviour, IMortal
{
    public Vector2Int lastMove;

    public bool soul = true;

    public virtual bool Flying => false;
    public virtual bool HasSoul => soul;
    public virtual bool SoulVulnerable => HasSoul;
    public virtual int Money => 1;
    public virtual bool Movable => true;
    public virtual bool GhostForm => false;

    public virtual bool Vulnerable => GetComponent<Invulnerability>().Current == 0;
    public virtual bool Disarmed => GetComponent<Disarm>().Current == 0;

    public virtual bool ShowInvulnerability => true;

    public virtual bool Threatening => OccupiesPlace;
    public virtual bool OccupiesPlace => alive;

    public virtual bool BenefitsFromTerrain => true;

    public Figure figure;

    public bool alive = true;

    public int movesSinceLastHit = 100500;

    public List<Func<MoveAction, Task>> afterTakeAction = new List<Func<MoveAction, Task>>();

    public virtual void OnGameStart() {
    }

    public virtual void Awake() {
        if (figure == null) figure = GetComponent<Figure>();
    }

    public virtual async Task Attack(Attack attack) {
        await attack.to.GetComponent<Unit>().Hit(attack);
        await Task.WhenAll(attack.afterAttack.Select(listener => listener()));
    }

    public virtual async Task Hit(Attack attack) {
        if (this == null) {
            Game.Debug($"Unit <null> hit by {attack}");
            return;
        }
        movesSinceLastHit = 0;
        Game.Debug($"{gameObject.name} hit by {attack}");
        await GetComponent<Health>().Hit(attack.damage);
    }

    protected virtual async Task BeforeDie() {
    }

    protected virtual async Task AfterDie() {
    }

    public virtual async Task Die() {
        if (!alive) {
            return;
        }
        Debug.LogFormat($"[{Game.instance.time}] {gameObject} at ({figure.Location.position}) dies");
        await BeforeDie();
        alive = false;

        gameObject.SetActive(false);

        foreach (var listener in GameEvents.instance.onUnitDeath.ToList()) {
            await listener(this);
        }

        await AfterDie();

        if (Game.instance.monsters.Contains(GetComponent<Monster>())) {
            Game.instance.monsters.Remove(GetComponent<Monster>());
            Game.Debug($"Monster {gameObject} at ({figure.Location}) removed from queue after death");
        }
        Destroy(gameObject);
    }
}
