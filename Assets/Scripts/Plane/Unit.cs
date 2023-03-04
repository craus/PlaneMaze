using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Unit : MonoBehaviour, IMortal
{
    public Vector2Int lastMove;

    public virtual bool Flying => false;
    public virtual bool HasSoul => true;
    public virtual int Money => 1;
    public virtual bool Movable => true;
    public virtual bool GhostForm => false;

    public virtual bool Vulnerable => GetComponent<Invulnerability>().Current == 0;
    public virtual bool Disarmed => GetComponent<Disarm>().Current == 0;
    public virtual bool ShowInvulnerability => true;

    public virtual bool OccupiesPlace => alive;

    public Figure figure;

    public bool alive = true;

    public virtual void OnGameStart() {
    }

    public virtual void Awake() {
        if (figure == null) figure = GetComponent<Figure>();
    }

    public virtual async Task Hit(Attack attack) {
        if (this == null) {
            return;
        }
        Debug.LogFormat($"[{Game.instance.time}] {gameObject.name} hit by {attack}");
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
        Debug.LogFormat($"[{Game.instance.time}] {gameObject} at ({figure.location.position}) dies");
        await BeforeDie();
        alive = false;

        foreach (var listener in GameEvents.instance.onUnitDeath.ToList()) {
            await listener(this);
        }

        await AfterDie();

        Game.instance.monsters.Remove(GetComponent<Monster>());
        Debug.LogFormat($"[{Game.instance.time} Monster {gameObject} at ({figure.location}) removed from queue after death");
        Destroy(gameObject);
    }
}
