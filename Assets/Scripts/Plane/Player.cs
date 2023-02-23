﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Player : Unit
{
    public static Player instance => Game.instance.player;

    public int totalGems;
    public int gems;

    public Wall wallSample;
    public Building markSample;

    public bool ongoingAnimations = false;

    public Queue<Vector2Int> commands = new Queue<Vector2Int>();

    public void Take(Item item) {
        item.Pick();
    }

    private async Task MoveTakeActions(Vector2Int delta) {
        var time = Game.instance.time;
        lastMove = delta;

        if ((await Task.WhenAll(Inventory.instance.items.Select(item => item.BeforeWalk(delta)))).Any(b => b)) {
            return;
        }

        if (await figure.TryWalk(delta)) {
            return;
        }

        if ((await Task.WhenAll(Inventory.instance.items.Select(item => item.AfterFailedWalk(delta)))).Any(b => b)) {
            return;
        }

        await figure.FakeMove(delta);
    }

    private async Task MoveInternal(Vector2Int delta) {
        if (GetComponent<MovesReserve>().Current < 0) {
            await GetComponent<MovesReserve>().Haste(1);
        } else {
            await MoveTakeActions(delta);
        }
        if (!alive) {
            return;
        }
        if (this == null) {
            return;
        }
        if (GetComponent<MovesReserve>().Current > 0) {
            await GetComponent<MovesReserve>().Freeze(1);
        } else {
            await Game.instance.AfterPlayerMove();
        }
        if (!alive) {
            return;
        }
        await GetComponent<Invulnerability>().Spend(1);
    }

    private async void Move(Vector2Int delta) {
        if (ongoingAnimations == true) {
            commands.Enqueue(delta);
            return;
        }
        ongoingAnimations = true;
        await MoveInternal(delta);
        ongoingAnimations = false;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Move(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Move(Vector2Int.right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Move(Vector2Int.left);
        }
        if (ongoingAnimations == false && commands.Count > 0) {
            Move(commands.Dequeue());
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            Build(figure.location, wallSample);
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            Build(figure.location, markSample);
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            DestroyBuilding(figure.location);
        }
    }

    public void Build(Cell place, Building sample) {
        if (gems < sample.cost) {
            return;
        }
        if (place.figures.Any(p => p.GetComponent<Building>())) {
            return;
        }
        gems -= sample.cost;
        Instantiate(sample, place.board.figureParent).GetComponent<Figure>().Move(place);
    }

    public void DestroyBuilding(Cell place) {
        var building = place.GetFigure<Building>();
        if (building == null) {
            return;
        }
        if (building.undestructible) {
            return;
        }
        gems += building.SellCost;
        Destroy(building.gameObject);
    }

    public override async Task Hit(Attack attack) {
        if (this == null) {
            return;
        }
        await Task.WhenAll(
            Inventory.instance.items
                .Select(item => item.GetComponent<IReceiveAttackModifier>())
                .Where(x => x != null)
                .OrderBy(x => x.Priority)
                .Select(x => x.ModifyAttack(attack))
        );

        await GetComponent<Health>().Hit(attack.damage);
    }
}
