using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Player : Unit
{
    public static Player instance => Game.instance ? Game.instance.player : null;

    public int totalGems;
    public int gems;

    public int damage = 1;

    public Wall wallSample;
    public Building markSample;

    public bool ongoingAnimations = false;

    public Queue<Vector2Int> commands = new Queue<Vector2Int>();

    public bool permanentInvulnerability = false;

    public override bool Vulnerable => base.Vulnerable && !permanentInvulnerability;

    public async Task Take(Item item) {
        await item.Pick();
    }

    private async Task MoveTakeActions(Vector2Int delta) {
        Debug.LogFormat($"[{Game.instance.time}] Player move take actions {delta}");
        lastMove = delta;

        for (int priority = 0; priority < 2; priority++) {
            if ((await Task.WhenAll(Inventory.instance.items.Select(item => item.BeforeWalk(delta, priority)))).Any(b => b)) {
                Debug.LogFormat($"[{Game.instance.time}] Player move end: before walk");
                return;
            }
        }

        if (await figure.TryWalk(delta, c => c.Free && (c.GetFigure<PaidCell>() == null || c.GetFigure<PaidCell>().price <= gems))) {
            Debug.LogFormat($"[{Game.instance.time}] Player move end: walk");
            return;
        }

        for (int priority = 0; priority < 2; priority++) {
            if ((await Task.WhenAll(Inventory.instance.items.Select(item => item.AfterFailedWalk(delta, priority)))).Any(b => b)) {
                Debug.LogFormat($"[{Game.instance.time}] Player move end: after failed walk");
                return;
            }
        }

        if (await DefaultAttack(delta)) {
            Debug.LogFormat($"[{Game.instance.time}] Player move end: default attack");
            return;
        }

        await figure.FakeMove(delta);
        Debug.LogFormat($"[{Game.instance.time}] Player move end: fake move");
    }

    private async Task<bool> DefaultAttack(Vector2Int delta) {
        var target = figure.location.Shift(delta).GetFigure<Unit>(u => u.Vulnerable);

        if (target == null || !target.Movable) {
            Debug.LogFormat($"[{Game.instance.time}] DefaultAttack failed: invalid target");
            return false;
        }
        if (!Game.CanAttack(this, target, null)) {
            Debug.LogFormat($"[{Game.instance.time}] DefaultAttack failed: cannot attack target");
            return false;
        }

        await figure.FakeMove(delta);

        if (target.figure.location.Shift(delta).Free) {
            SoundManager.instance.push.Play();
            await target.figure.TryWalk(delta);
            Debug.LogFormat($"[{Game.instance.time}] DefaultAttack push");
        } else {
            SoundManager.instance.pushAttack.Play();
            await DealDamage(target);
            Debug.LogFormat($"[{Game.instance.time}] DefaultAttack deal damage");
        }
        return true;
    }

    public async Task DealDamage(Unit target) {
        var currentDamage = damage;
        if (Inventory.instance.GetItem<RingOfStrength>()) {
            currentDamage++;
        }
        await target.Hit(new Attack(figure, target.figure, currentDamage));
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
        if (this == null || !alive) {
            return;
        }
        await GetComponent<Invulnerability>().Spend(1);
    }

    private async void Move(Vector2Int delta) {
        if (
            Game.instance.startPanel.activeSelf || 
            Game.instance.winPanel.activeSelf || 
            Game.instance.losePanel.activeSelf ||
            InfoPanel.instance.panel.activeSelf
        ) {
            Game.instance.ClosePanel();
            return;
        }
        if (ongoingAnimations == true) {
            commands.Enqueue(delta);
            return;
        }
        ongoingAnimations = true;
        await MoveInternal(delta);
        ongoingAnimations = false;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            Move(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            Move(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            Move(Vector2Int.right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            Move(Vector2Int.left);
        }
        if (ongoingAnimations == false && commands.Count > 0) {
            Move(commands.Dequeue());
        }

        if (Cheats.on) {
            if (Input.GetKeyDown(KeyCode.G)) {
                gems++;
            }

            if (Input.GetKeyDown(KeyCode.I)) {
                permanentInvulnerability ^= true;
            }
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
        Debug.LogFormat($"[{Game.instance.time}] Player hit by {attack}");
        await Task.WhenAll(
            Inventory.instance.items
                .Select(item => item.GetComponent<IReceiveAttackModifier>())
                .Where(x => x != null)
                .OrderBy(x => x.Priority)
                .Select(x => x.ModifyAttack(attack))
        );

        await GetComponent<Health>().Hit(attack.damage);
    }

    public override void Awake() {
        base.Awake();
        figure.afterBoardChange.Add(AfterBoardChange);
    }

    private async Task AfterBoardChange(Board from, Board to) {
        if (from != null) {
            from.gameObject.SetActive(false);
        }
        if (to != null) {
            to.gameObject.SetActive(true);
        }

        if (to == Game.instance.mainWorld) {
            MusicManager.instance.Switch(MusicManager.instance.playlist);
        } else {
            MusicManager.instance.Switch(MusicManager.instance.storePlaylist);
        }
    }

    public override async Task Die() {
        await base.Die();
        await Game.instance.Lose();
    }
}
