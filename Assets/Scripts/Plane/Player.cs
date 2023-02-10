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

    public override void AfterMove(Cell from, bool isTeleport) {
        if (!isTeleport) {
            if (from != figure.location) {
                if (figure.location.fieldCell.teleport) {
                    figure.Move(figure.location.board.GetCell(figure.location.fieldCell.teleportTarget), isTeleport: true);
                }
            }
        }
    }

    public void Take(Item item) {
        item.Pick();
    }

    private async Task MoveTakeActions(Vector2Int delta) {
        var time = Game.instance.time;
        if (await figure.TryWalk(delta)) {
            return;
        }
        Debug.LogFormat($"[{time}] Failed walk");

        if ((await Task.WhenAll(Inventory.instance.items.Select(item => item.AfterFailedWalk(delta)))).Any(b => b)) {
            Debug.LogFormat($"[{time}] Item used on failed walk");
            return;
        }
        Debug.LogFormat($"[{time}] Failed items use on failed walk");

        await figure.FakeMove(delta);
    }

    private async void Move(Vector2Int delta) {
        await MoveTakeActions(delta);
        await Game.instance.AfterPlayerMove();
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
        Instantiate(sample, Game.instance.figureParent).GetComponent<Figure>().Move(place);
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
}
