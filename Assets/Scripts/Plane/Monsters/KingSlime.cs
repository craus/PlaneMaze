using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class KingSlime : Monster
{
    public int size = 1;
    public int childrenCount = 2;

    public int cooldown;
    public int currentCooldown;

    [SerializeField] private List<Monster> minions;

    [SerializeField] private Slime childSample;
    [SerializeField] private Transform slimeSizeTransform;

    public override bool SoulVulnerable => true;
    public override bool PoisonImmune => true;

    public GameObject activeModel;

    public override bool HasSoul => base.HasSoul && size == 0;
    public override int Money => size == 0 ? 1 : 0;

    public List<Slime> deadChildren = new List<Slime>();

    public override void Awake() {
        base.Awake();
        Init(); 
        
        new ValueTracker<int>(() => currentCooldown, v => {
            currentCooldown = v;
            UpdateSprite();
        });

        new ValueTracker<List<Slime>>(() => deadChildren.ToList(), v => deadChildren = v.ToList());
    }

    public void Init() {
        damage = 1 + size;
        cooldown = 1 + size;
        currentCooldown = cooldown;
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
        activeModel.SetActive(currentCooldown <= 1);
    }

    private void SpawnMinion(Cell cell) {
        Game.GenerateFigure(cell, minions.rnd());
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

    protected override async Task MakeMove() {
        --currentCooldown;
        UpdateSprite();
        if (currentCooldown > 0) {
            return;
        }

        Vector2Int playerDelta = Player.instance.figure.Location.position - figure.Location.position;

        if (playerDelta.SumDelta() == 1) {
            if (!await TryAttack(playerDelta)) {
                await figure.FakeMove(playerDelta);
            }
        } else {
            var delta = Helpers.Moves.Rnd();
            if (!await TryAttack(delta)) {
                if (!await SmartWalk(delta)) {
                    await figure.FakeMove(delta);
                }
            }
        }
        if (this == null || !alive) {
            return;
        }

        currentCooldown = cooldown;
        UpdateSprite();
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
