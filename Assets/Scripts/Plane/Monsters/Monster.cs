﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public abstract class Monster : Unit, IMovable
{
    public virtual bool FreeCell(Cell cell) => cell.Free;

    public int movesSinceLastHeal = 100500;

    public int movesSinceHitToHeal = 3;
    public int healCooldown = 3;

    public override bool BenefitsFromTerrain => base.BenefitsFromTerrain && GameManager.instance.metagame.Ascention<MonstersBenefitFromTerrain>();

    public override void Awake() {
        base.Awake();
        new ValueTracker<int>(() => movesSinceHitToHeal, v => {
            movesSinceHitToHeal = v;
        });
    }

    protected async Task<bool> SmartWalk(Vector2Int delta) {
        if (!Flying && figure.Location.Shift(delta).GetFigure<WolfTrap>() != null) {
            return false;
        }
        return await figure.TryWalk(delta, FreeCell);
    }

    protected async Task<bool> WalkOrFakeMove(Vector2Int delta) {
        if (await figure.TryWalk(delta)) {
            return true;
        } else {
            await figure.FakeMove(delta);
            return false;
        }
    }

    protected async Task<bool> SmartWalkOrFakeMove(Vector2Int delta) {
        if (await SmartWalk(delta)) {
            return true;
        } else {
            await SmartFakeMove(delta);
            return false;
        }
    }

    protected async Task<bool> SmartFakeMove(Vector2Int delta) {
        //if (figure.location.Shift(delta).GetFigure<WolfTrap>() != null) {
        //    return false;
        //}
        return await figure.FakeMove(delta);
    }

    public GameObject attackProjectile;
    public int damage = 1;

    public virtual async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = figure.Location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            return await Attack(newPosition.GetFigure<Player>());
        }
        return false;
    }

    public virtual void PlayAttackSound() => SoundManager.instance.monsterMeleeAttack.Play();

    public virtual async Task BeforeAttack(Vector2Int delta) { }
    public virtual async Task AfterAttack(Vector2Int delta) { }

    public virtual Cell AttackLocation(Vector2Int delta, Unit target) => figure.Location;
    public virtual Cell DefenceLocation(Vector2Int delta, Unit target) => target.figure.Location;

    public async Task<bool> Attack(Unit target, Vector2Int? maybeDelta = null) {
        var delta = maybeDelta ?? Helpers.StepAtDirection(target.figure.Location.position - figure.Location.position);
        if (!Game.CanAttack(this, target, null, AttackLocation(delta, target), DefenceLocation(delta, target))) {
            return false;
        }

        target.figure.Location.OnOccupyingUnitAttacked(target);

        PlayAttackSound();

        GetComponent<DangerSprite>().sprite.enabled = true;
        await figure.FakeMove(target.figure.Location.position - figure.Location.position);
        if (target == null || !target.alive) {
            return true;
        }

        var ap = Instantiate(attackProjectile, Game.instance.transform);
        ap.gameObject.SetActive(true); // object was inactive for unknown reason
        ap.transform.position = target.transform.position;

        await Helpers.Delay(0.1f);

        Game.instance.lastAttackedMonster = this;
        await target.Hit(new Attack(delta, figure, target.figure, AttackLocation(delta, target), DefenceLocation(delta, target), damage));

        if (ap != null) {
            Destroy(ap);
        }

        return true;
    }

    protected virtual List<Vector2Int> Moves => new List<Vector2Int>() {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
    };

    protected Vector2Int PlayerDelta => Player.instance.figure.Location.position - figure.Location.position;

    protected virtual async Task MakeMove() {
    }

    private async Task Regenerate() {
        if (Metagame.instance.Ascention<MonstersHeal>()) {
            if (
                movesSinceLastHit >= movesSinceHitToHeal &&
                movesSinceLastHeal >= healCooldown
            ) {
                await GetComponent<Health>().Heal(1);
                movesSinceLastHeal = 0;
            }
        }
    }

    private async Task MoveInternal() {
        movesSinceLastHeal++;
        movesSinceLastHit++;
        await Regenerate();
        await MakeMove();
        if (this == null) return;
        await GetComponent<Disarm>().Spend(1);
        await GetComponent<Root>().Spend(1);
        await GetComponent<Curse>().Spend(1);
    }

    public async Task Move() {
        if (this == null || !this.alive) {
            Debug.LogError("Dead monster moves");
            return;
        }
        GetComponent<DangerSprite>().sprite.enabled = false;
        var figureLocation = figure.Location;
        var board = figureLocation.board;
        var player = Player.instance;
        var playerFigure = player.figure;
        var playerLocation = playerFigure.Location;
        var playerBoard = playerLocation.board;
        if (!alive || figure.Location.board != Player.instance.figure.Location.board) {
            return;
        }
        await GetComponent<Invulnerability>().Spend(1);
        if (GetComponent<MovesReserve>().Current < 0) {
            await GetComponent<MovesReserve>().Haste(1);
            return;
        }
        await MoveInternal();

        if (!alive) {
            return;
        }
        for (int i = 0; i < 10 && this != null && GetComponent<MovesReserve>().Current > 0; i++) {
            await GetComponent<MovesReserve>().Freeze(1);
            await MoveInternal();
            if (!alive) {
                return;
            }
        }
        if (Metagame.instance.Ascention<FasterMonsters>()) {
            if (Rand.rndEvent(0.1f)) {
                await GetComponent<MovesReserve>().Haste(1);
            }
        }
    }

    protected override async Task AfterDie() {
        var reward = Money + (Money > 0 && Inventory.instance.GetItem<RingOfMidas>() != null ? 1 : 0);
        Game.instance.AddGem(figure.Location, reward);
    }

    public void OnDestroy() {
        if (Game.instance != null && Game.instance.movables.Contains(GetComponent<Monster>())) {
            Game.instance.movables.Remove(GetComponent<Monster>());
            Game.Debug($"Monster {gameObject} at ({figure.Location}) removed from queue after death");
        }
    }
}
