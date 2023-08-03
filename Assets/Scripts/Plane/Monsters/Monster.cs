using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public abstract class Monster : Unit
{
    public virtual bool FreeCell(Cell cell) => cell.Free;

    public int movesSinceLastHeal = 100500;

    public int movesSinceHitToHeal = 3;
    public int healCooldown = 3;

    public override bool BenefitsFromTerrain => base.BenefitsFromTerrain && GameManager.instance.metagame.Ascention<MonstersBenefitFromTerrain>();

    protected async Task<bool> SmartWalk(Vector2Int delta) {
        if (!Flying && figure.location.Shift(delta).GetFigure<WolfTrap>() != null) {
            return false;
        }
        return await figure.TryWalk(delta, FreeCell);
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
        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            return await Attack(newPosition.GetFigure<Player>());
        }
        return false;
    }

    public virtual void PlayAttackSound() => SoundManager.instance.monsterMeleeAttack.Play();

    public virtual async Task BeforeAttack(Vector2Int delta) { }
    public virtual async Task AfterAttack(Vector2Int delta) { }

    public virtual Cell AttackLocation(Vector2Int delta, Unit target) => figure.location;
    public virtual Cell DefenceLocation(Vector2Int delta, Unit target) => target.figure.location;

    public async Task<bool> Attack(Unit target, Vector2Int? maybeDelta = null) {
        var delta = maybeDelta ?? Helpers.StepAtDirection(target.figure.location.position - figure.location.position);
        if (!Game.CanAttack(this, target, null, AttackLocation(delta, target), DefenceLocation(delta, target))) {
            return false;
        }

        PlayAttackSound();

        GetComponent<DangerSprite>().sprite.enabled = true;
        await figure.FakeMove(target.figure.location.position - figure.location.position);
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

    protected virtual async Task MakeMove() {
    }

    private async Task MoveInternal() {
        await GetComponent<Disarm>().Spend(1);
        movesSinceLastHeal++;
        movesSinceLastHit++;

        if (Metagame.instance.Ascention<MonstersHeal>()) {
            if (
                movesSinceLastHit >= movesSinceHitToHeal &&
                movesSinceLastHeal >= healCooldown
            ) {
                await GetComponent<Health>().Heal(1);
                movesSinceLastHeal = 0;
            }
        }

        await MakeMove();
    }

    public async Task Move() {
        if (this == null) {
            Debug.LogError("Dead monster moves");
        }
        GetComponent<DangerSprite>().sprite.enabled = false;
        var figureLocation = figure.location;
        var board = figureLocation.board;
        var player = Player.instance;
        var playerFigure = player.figure;
        var playerLocation = playerFigure.location;
        var playerBoard = playerLocation.board;
        if (!alive || figure.location.board != Player.instance.figure.location.board) {
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
        Game.instance.AddGem(figure.location, reward);
    }

    public void OnDestroy() {
        if (Game.instance != null && Game.instance.monsters.Contains(GetComponent<Monster>())) {
            Game.instance.monsters.Remove(GetComponent<Monster>());
            Game.Debug($"Monster {gameObject} at ({figure.location}) removed from queue after death");
        }
    }
}
