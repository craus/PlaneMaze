﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class KingSlime : Monster
{
    public int size = 1;
    public int childrenCount = 2;

    [SerializeField] private List<Monster> minions;
    [SerializeField] private GelatinousCube gelatinousCubeSample;

    [SerializeField] private Slime childSample;
    [SerializeField] private Transform slimeSizeTransform;

    public override bool PoisonImmune => true;

    public GameObject activeModel;

    public override bool HasSoul => false;
    public override int Money => size == 0 ? 1 : 0;

    [SerializeField] private List<Transform> targets;

    public List<Slime> deadChildren = new List<Slime>();

    [SerializeField] private List<Cell> chargedArea = null;

    public override void Awake() {
        base.Awake();
        Init(); 
        
        new ValueTracker<List<Slime>>(() => deadChildren.ToList(), v => deadChildren = v.ToList());

        new ValueTracker<List<Cell>>(() => chargedArea, v => {
            chargedArea = v;
            UpdateIcons();
        });
    }

    private void UpdateIcons() {
        if (chargedArea == null) {
            targets.ForEach(t => t.gameObject.SetActive(false));
            return;
        }
        for (int i = 0; i < targets.Count; i++) {
            if (i < chargedArea.Count()) {
                targets[i].gameObject.SetActive(true);
                targets[i].position = chargedArea[i].transform.position.Change(z: targets[i].position.z);
            } else {
                targets[i].gameObject.SetActive(false);
            }
        }
    }

    public void Init() {
        damage = 1 + size;
        UpdateSprite();
    }

    private float sizeMultiplier {
        get {
            if (size == 3) return 2.5f;
            return 0.7f + 0.3f * size;
        }
    }

    private void UpdateSprite() {
        slimeSizeTransform.localScale = (sizeMultiplier * Vector3.one).Change(z: 1);
        activeModel.SetActive(true);
    }

    private void SpawnMinion(Cell cell) {
        var minion = Game.GenerateFigure(cell, minions.rnd());
        minion.poor = true;
    }

    private void SpawnGelatinousCube(Cell cell) {
        Game.GenerateFigure(cell, gelatinousCubeSample);
    }

    public async override Task Hit(Attack attack) {
        await base.Hit(attack);
        var delta = PlayerDelta;

        if (delta.MaxDelta() == 1) {
            if (!await Player.instance.figure.TryWalk(delta)) {
                await Attack(Player.instance);
            }
        }

        figure.Location.Vicinity(-1, 2, -1, 2).ForEach(cell => {
            if (cell.Free) {
                SpawnMinion(cell);
            }
        });
    }

    private async Task<bool> TryMoveAway(Unit target) {
        if (target.figure.size != 1) return false;

        var oldPosition = target.figure.Location;
        var newPosition = target.figure.Location.Neighbours().Where(cell => !chargedArea.Contains(cell) && cell.FreeFor(target)).Rnd();
        
        if (newPosition == null) return false;

        await target.figure.Move(newPosition);

        SpawnGelatinousCube(oldPosition);

        return true;
    }

    private async Task MoveAwayOrAttack(Unit target) {
        if (await TryMoveAway(target)) return;

        await Attack(target);
    }

    private IEnumerable<Cell> DirectAttackArea(Vector2Int playerDelta) =>
        new List<Cell> {
            ClosestPointToPlayer.Shift(playerDelta),
            ClosestPointToPlayer.Shift(2*playerDelta)
        };

    private async Task ChargeAttack(IEnumerable<Cell> targetArea) {
        SoundManager.instance.chargeMagicAttack.Play();
        chargedArea = targetArea.ToList();
        UpdateIcons();
    }

    protected override async Task MakeMove() {
        // Execute Attack
        if (chargedArea != null && chargedArea.Count > 0) {
            Debug.LogFormat($"{this} attack charged area");
            await Task.WhenAll(
                chargedArea.SelectMany(c => c.GetFigures<Unit>(u => u.SoulVulnerable)).ToList().Select(u => MoveAwayOrAttack(u)).
                Concat(chargedArea.Select(FakeAttack))
            );
            chargedArea = null;
            UpdateIcons();
            return;
        }

        var playerDelta = PlayerDelta;
        var closestPointToPlayer = ClosestPointToPlayer;

        Debug.LogFormat($"playerDelta = {playerDelta}");

        if (playerDelta.SumDelta() == 1) {
            if (await figure.CheckWalk(playerDelta, free: cell => cell.FreeExcept(this, Player.instance))) {

                Debug.LogFormat($"Push attack");
                if (!await Player.instance.figure.CheckWalk(playerDelta)) {
                    Debug.LogFormat($"Push attack - attack");
                    await Attack(Player.instance);
                } else {
                    Debug.LogFormat($"Push attack - push");
                    var movePlayer = Player.instance.figure.TryWalk(playerDelta);
                    var moveSelf = figure.TryWalk(playerDelta);
                    await Task.WhenAll(movePlayer, moveSelf);
                }
                return;
            }
        }

        if (playerDelta.MaxDelta() <= 2 && playerDelta.MinDelta() == 0) {
            playerDelta = playerDelta.StepAtDirection();
            Debug.LogFormat($"Check direct attack charge: playerDelta = {playerDelta}");
            if (
                !DirectAttackArea(playerDelta).Any(cell => cell.GetFigure<Unit>(u => u != Player.instance) != null)
            ) {
                Debug.LogFormat($"ChargeAttack: playerDelta = {playerDelta}");
                await ChargeAttack(DirectAttackArea(playerDelta));
                return;
            }
        }

        if (playerDelta.MaxDelta() <= 4) {
            Debug.LogFormat($"Check spawn minion");
            var targetCell = figure.OccupiedArea(figure.Location)
                .SelectMany(cell => cell.Neighbours())
                .Where(cell => cell.Free).Rnd();

            if (targetCell != null) {
                Debug.LogFormat($"Spawn minion: {targetCell}");
                SpawnMinion(targetCell);
                return;
            }
        }

        // Wander randomly
        Debug.LogFormat($"{this} wanders randomly");
        var delta = Helpers.Moves.Rnd();
        if (!await SmartWalk(delta)) {
            await figure.FakeMove(delta);
        }
    }

    public async Task CheckWin(Slime child) {
        deadChildren.Add(child);
        if (deadChildren.Count == 16) {
            var win = Game.instance.Win();
        }
    }

    protected async override Task AfterDie() {
        await base.AfterDie();

        figure.OccupiedArea(figure.Location).ForEach(async cell => {
            var child = Game.GenerateFigure(cell, childSample); 
            child.size = size - 1;
            child.king = this;
            await child.GetComponent<MovesReserve>().Freeze(1);
            child.Init();
        });
    }
}
