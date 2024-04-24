using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Slime : Monster
{
    public int size = 1;
    public int childrenCount = 2;
    public KingSlime king = null;

    public int cooldown;
    public int currentCooldown;

    [SerializeField] private Slime childSample;
    [SerializeField] private Transform slimeSizeTransform;

    public override bool SoulVulnerable => true;
    public override bool PoisonImmune => true;

    public GameObject activeModel;
    public GameObject crown;

    public override bool HasSoul => base.HasSoul && king == null && size == 0;
    public override int Money => size == 0 && king == null ? 1 : 0;

    public override void Awake() {
        base.Awake();
        Init(); 
        
        new ValueTracker<int>(() => currentCooldown, v => {
            currentCooldown = v;
            UpdateSprite();
        });
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
        crown.SetActive(king != null);
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

    protected async override Task AfterDie() {
        await base.AfterDie();
        if (size == 0) {
            if (king != null) {
                await king.CheckWin(this);
            }
            return;
        }
        foreach (
            var p in 
            Rand.RndSelection(figure.Location.SmallestVicinity(v => v.Count(c => c.Free) >= childrenCount)
            .Where(c => c.Free), childrenCount)
        ) {
            var child = Game.GenerateFigure(p, childSample);
            child.size = size - 1;
            child.king = king;
            await child.GetComponent<MovesReserve>().Freeze(1);
            child.Init();
        }
    }
}
