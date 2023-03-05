using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Slime : Monster
{
    public int size = 1;
    public int childrenCount = 2;

    public int cooldown;
    public int currentCooldown;

    [SerializeField] private Slime childSample;
    [SerializeField] private Transform slimeSizeTransform;

    public override bool SoulVulnerable => true;

    public GameObject activeModel;

    public override bool HasSoul => base.HasSoul && size == 0;
    public override int Money => size == 0 ? 1 : 0;

    public override void Awake() {
        base.Awake();
        Init();
    }

    public void Init() {
        damage = 1 + size;
        cooldown = 1 + size;
        currentCooldown = cooldown;
        GetComponent<Health>().current = GetComponent<Health>().max = 1 + size;
        UpdateSprite();
    }

    private void UpdateSprite() {
        slimeSizeTransform.localScale = ((0.7f + 0.3f * size) * Vector3.one).Change(z: 1);
        activeModel.SetActive(currentCooldown <= 1);
    }

    protected override async Task MakeMove() {
        --currentCooldown;
        UpdateSprite();
        if (currentCooldown > 0) {
            return;
        }

        Vector2Int playerDelta = Player.instance.figure.location.position - figure.location.position;

        if (playerDelta.SumDelta() == 1) {
            if (!await TryAttack(playerDelta)) {
                await figure.FakeMove(playerDelta);
            }
        } else {
            var delta = moves.Rnd();
            if (!await TryAttack(delta)) {
                if (!await SmartWalk(delta)) {
                    await figure.FakeMove(delta);
                }
            }
        }

        currentCooldown = cooldown;
        UpdateSprite();
    }

    protected async override Task AfterDie() {
        await base.AfterDie();
        if (size == 0) {
            return;
        }
        foreach (
            var p in 
            Rand.RndSelection(figure.location.SmallestVicinity(v => v.Count(c => c.Free) >= childrenCount)
            .Where(c => c.Free), childrenCount)
        ) {
            var child = Game.instance.GenerateFigure(p, childSample);
            child.size = size - 1;
            await child.GetComponent<MovesReserve>().Freeze(1);
            child.Init();
        }
    }
}
