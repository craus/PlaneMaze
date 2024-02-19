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
    public static bool insideFog = false;
    public static bool trueSight = false;

    public override bool TrueSight => base.TrueSight || PlaneMaze.Cheats.instance.trueSight;

    public int totalGems;
    public int gems;

    public int damage = 1;

    public Wall wallSample;
    public Building markSample;

    public bool ongoingAnimations = false;

    public Queue<Vector2Int> commands = new Queue<Vector2Int>();

    public bool permanentInvulnerability = false;

    public override bool Vulnerable => base.Vulnerable && !permanentInvulnerability;

    public override bool BenefitsFromTerrain => base.BenefitsFromTerrain && !GameManager.instance.metagame.HasAscention<PlayerDontBenefitFromTerrain>();

    public async Task Take(Item item) {
        await item.Pick();
    }

    private async Task AfterTakeAction(MoveAction action) {
        await Task.WhenAll(afterTakeAction.Select(listener => listener(action)));
    }

    /// <summary>
    /// priority 0 - rings
    /// priority 1 - weapons
    /// priority 2 - boots
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    private async Task MoveTakeActions(Vector2Int delta) {
        Game.Debug($"Player move take actions {delta}");
        lastMove = delta;

        for (int priority = 0; priority < 3; priority++) {
            if (await Inventory.instance.items.ToList().Select(item => item.BeforeWalk(delta, priority)).AnyInSequence()) {
                await AfterTakeAction(new UnknownAction());
                Game.Debug($"Player move end: before walk");
                return;
            }
        }

        var oldLocation = figure.Location;
        var newLocation = oldLocation.Shift(delta);
        if (await figure.TryWalk(delta, c => c.Free && (c.GetFigure<PaidCell>() == null || c.GetFigure<PaidCell>().price <= gems))) {
            await AfterTakeAction(new Walk(oldLocation, newLocation));
            Game.Debug($"Player move end: walk");
            return;
        }

        for (int priority = 0; priority < 3; priority++) {
            if (await Inventory.instance.items.ToList().Select(item => item.AfterFailedWalk(delta, priority)).AnyInSequence()) {
                await AfterTakeAction(new UnknownAction());
                Game.Debug($"Player move end: after failed walk");
                return;
            }
        }

        if (await DefaultAttack(delta)) {
            await AfterTakeAction(new UnknownAction());
            Game.Debug($"Player move end: default attack");
            return;
        }

        SoundManager.instance.failedAction.Play();
        await figure.FakeMove(delta);
        await AfterTakeAction(new FailedMove(delta));
        Game.Debug($"Player move end: fake move");
    }

    private async Task<bool> DefaultAttack(Vector2Int delta) {
        var target = figure.Location.Shift(delta).GetFigure<Unit>(u => u.Vulnerable);

        if (target == null || !target.Movable) {
            Debug.LogFormat($"[{Game.instance.time}] DefaultAttack failed: invalid target");
            return false;
        }
        if (!Game.CanAttack(this, target, null)) {
            Debug.LogFormat($"[{Game.instance.time}] DefaultAttack failed: cannot attack target");
            return false;
        }
        target.figure.Location.OnOccupyingUnitAttacked(target);
        await figure.FakeMove(delta);

        if (target.figure.Location.Shift(delta).Free) {
            SoundManager.instance.push.Play();
            await target.figure.TryWalk(delta);
            Debug.LogFormat($"[{Game.instance.time}] DefaultAttack push");
        } else {
            SoundManager.instance.pushAttack.Play();
            await Attack(new Attack(delta, figure, target.figure, figure.Location, target.figure.Location, damage));
            Debug.LogFormat($"[{Game.instance.time}] DefaultAttack deal damage");
        }
        return true;
    }

    public async Task DealDamage(Vector2Int delta, Unit target) {
        var currentDamage = damage;
        if (Inventory.instance.GetItem<RingOfStrength>()) {
            currentDamage++;
        }
        await target.Hit(new Attack(delta, figure, target.figure, figure.Location, target.figure.Location, currentDamage));
    }

    public override async Task Attack(Attack attack) {
        if (this == null) {
            return;
        }
        Debug.LogFormat($"[{Game.instance.time}] Player attack with {attack}");
        await Task.WhenAll(
            Inventory.instance.items
                .Select(item => item.GetComponent<IAttackModifier>())
                .Where(x => x != null)
                .OrderBy(x => x.Priority)
                .Select(x => x.ModifyAttack(attack))
        );

        await base.Attack(attack);
    }

    private async Task MoveInternal(Vector2Int delta) {
        if (GetComponent<MovesReserve>().Current < 0) {
            await GetComponent<MovesReserve>().Haste(1);
        } else {
            await MoveTakeActions(delta);
        }
        Game.instance.moveNumber++;
        if (this == null) {
            return;
        }
        if (GetComponent<MovesReserve>().Current > 0) {
            await GetComponent<MovesReserve>().Freeze(1);
        } else {
            await Game.instance.AfterPlayerMove();
        }
        if (this == null) {
            return;
        }
        await GetComponent<Disarm>().Spend(1);
        await GetComponent<Root>().Spend(1);
        await GetComponent<Curse>().Spend(1);
        await GetComponent<Curse>().Prepare();
        await GetComponent<Invulnerability>().Spend(1);

        UndoManager.instance.Save();
    }

    private async void Move(Vector2Int delta) {
        if (
            InfoPanel.instance.panel.activeSelf ||
            ConfirmationManager.instance.AwaitingConfirmation
        ) {
            if (InfoPanel.instance.panel.activeSelf) {
                InfoPanel.instance.panel.SetActive(false);
            }
            return;
        }
        Cursor.visible = false;
        if (ongoingAnimations == true) {
            commands.Enqueue(delta);
            return;
        }
        ongoingAnimations = true;
        await MoveInternal(delta);
        ongoingAnimations = false;
    }

    public void ReadKeys() {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift)) {
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
    }

    public void Update() {
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
        lastAttacker = attack.from.GetComponent<IAttacker>();
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
        figure.afterMove.Add(AfterMove);

        new ValueTracker<int>(() => gems, v => gems = v);
        new ValueTracker<int>(() => totalGems, v => totalGems = v);
        new ValueTracker<bool>(() => FogPlane.instance.model.activeSelf, FogPlane.instance.model.SetActive);
    }

    private async Task AfterMove(Cell from, Cell to) {
        GlobalInvisibilityCheck();
    }

    public void GlobalInvisibilityCheck() {
        Game.instance.GetComponentsInChildren<Invisibility>().ForEach(i => i.Check());
        FogPlane.instance.model.SetActive(figure.Location.GetFigure<Fog>(f => f.On));
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
            WorldGenerator.instance.RestockAllStores();
        } else {
            MusicManager.instance.Switch(MusicManager.instance.storePlaylist);
        }
    }

    protected override async Task AfterDie() {
        await base.AfterDie();
        var lose = Game.instance.Lose();
    }
}
