﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public abstract class Monster : Unit
{
    public virtual bool FreeCell(Cell cell) => cell.Free;

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

    public override void Awake() {
        base.Awake();
    }

    public virtual async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            return await Attack(newPosition.GetFigure<Player>());
        }
        return false;
    }

    public async Task<bool> Attack(Unit target) {
        if (!Game.CanAttack(this, target)) {
            return false;
        }

        var ap = Instantiate(attackProjectile);
        ap.gameObject.SetActive(true); // object was inactive for unknown reason
        ap.transform.position = target.transform.position;

        await Helpers.Delay(0.1f);

        await target.Hit(new Attack(figure.location, target.figure.location, damage));

        if (ap != null) {
            Destroy(ap);
        }

        return true;
    }

    protected List<Vector2Int> moves = new List<Vector2Int>() {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
    };

    protected virtual async Task MakeMove() {
    }

    public async Task Move() {
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
        await MakeMove();
        if (!alive) {
            return;
        }
        for (int i = 0; i < 10 && this != null && GetComponent<MovesReserve>().Current > 0; i++) {
            await GetComponent<MovesReserve>().Freeze(1);
            await MakeMove(); 
            if (!alive) {
                return;
            }
        }
    }

    protected override async Task AfterDie() {
        Player.instance.gems += Money + (Money > 0 && Inventory.instance.GetItem<RingOfMidas>() != null ? 1 : 0);
        Game.instance.monsters.Remove(this);
    }
}
