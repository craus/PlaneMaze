using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Unit : MonoBehaviour, IMortal, IAttacker
{
    public Vector2Int lastMove;

    public bool soul = true;

    public virtual bool Flying => false;
    public virtual bool HasSoul => soul;
    public virtual bool SoulVulnerable => HasSoul;
    public virtual int Money => 1;
    public virtual bool Movable => true;
    public virtual bool GhostForm => false;
    public virtual bool Boss => false;
    public virtual bool TrueSight => false;

    public virtual bool Vulnerable => GetComponent<Invulnerability>().Current == 0;
    public virtual bool Disarmed => GetComponent<Disarm>().Current == 0;

    public virtual bool ShowInvulnerability => true;

    public virtual bool Threatening => OccupiesPlace;
    public virtual bool OccupiesPlace => alive;

    public virtual bool BenefitsFromTerrain => true;

    public Figure figure;

    public bool alive = true;
    public bool dying = false;

    public int movesSinceLastHit = 100500;
    public IAttacker lastAttacker;

    public List<Func<MoveAction, Task>> afterTakeAction = new List<Func<MoveAction, Task>>();

    public virtual void OnGameStart() {
    }

    public virtual void Awake() {
        if (figure == null) figure = GetComponent<Figure>();

        new ValueTracker<bool>(() => alive, v => alive = v);
        new ValueTracker<bool>(() => dying, v => dying = v);
        new ValueTracker<bool>(() => soul, v => soul = v);
        new ValueTracker<int>(() => movesSinceLastHit, v => movesSinceLastHit = v);

        new ValueTracker<List<Func<MoveAction, Task>>>(() => afterTakeAction.ToList(), v => afterTakeAction = v.ToList());
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
        lastAttacker = attack.from.GetComponent<IAttacker>();
        Game.Debug($"{gameObject.name} hit by {attack}");
        await GetComponent<Health>().Hit(attack.damage);
    }

    protected virtual async Task BeforeDie() {
    }

    protected virtual async Task AfterDie() {
    }

    private void PlayDeathSound() {
        if (GetComponent<Player>() != null) {
            SoundManager.instance.playerDeath.Play();
        } else if (GetComponent<Lich>() != null) {
            SoundManager.instance.lichDeath.Play();
        } else if (GetComponent<Monster>() != null) {
            if (GetComponent<Tree>() != null || GetComponent<Coffin>() != null) {
                SoundManager.instance.woodCrash.Play();
            } else {
                SoundManager.instance.monsterDeath.Play();
            }
        }
    }

    public virtual async Task Die() {
        if (!alive || dying) {
            return;
        }
        dying = true;
        Debug.LogFormat($"[{Game.instance.time}] {gameObject} at ({figure.Location.position}) dies");

        PlayDeathSound();

        await BeforeDie();
        alive = false;

        gameObject.SetActive(false);

        foreach (var listener in GameEvents.instance.onUnitDeath.ToList()) {
            await listener(this);
        }

        await AfterDie();

        if (Game.instance.movables.Contains(GetComponent<Monster>())) {
            Game.instance.movables.Remove(GetComponent<Monster>());
            Game.Debug($"Monster {gameObject} at ({figure.Location}) removed from queue after death");
        }
        //Destroy(gameObject);
        GetComponent<Figure>().OnDestroy();
    }
}
