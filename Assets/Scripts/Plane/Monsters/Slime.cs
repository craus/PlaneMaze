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

    public override bool HasSoul => size == 0;
    public override int Money => size == 0 ? 1 : 0;

    public override void Awake() {
        base.Awake();
        Init();
    }

    public void Init() {
        damage = size;
        cooldown = 1 + size;
        currentCooldown = cooldown;
        GetComponent<Health>().current = GetComponent<Health>().max = 1 + size;
        UpdateSprite();
    }

    public async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            return await Attack(newPosition.GetFigure<Player>());
        }
        return false;
    }

    private void UpdateSprite() {
        slimeSizeTransform.localScale = 0.5f * (1 + size) * Vector3.one;
    }

    protected override async Task MakeMove() {
        --currentCooldown;
        if (currentCooldown > 0) {
            return;
        }

        Vector2Int playerDelta = Player.instance.figure.location.position - figure.location.position;

        if (playerDelta.SumDelta() == 1) {
            await TryAttack(playerDelta);
        } else {
            var delta = moves.Rnd();
            if (!await TryAttack(delta)) {
                if (!await SmartWalk(delta)) {
                    await figure.FakeMove(delta);
                }
            }
        }

        currentCooldown = cooldown;
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
            var child = Instantiate(childSample);
            Game.instance.monsters.Add(child);
            child.size = size - 1;
            await child.GetComponent<MovesReserve>().Freeze(1);
            child.Init();
            await child.figure.Move(p);
        }
    }
}
