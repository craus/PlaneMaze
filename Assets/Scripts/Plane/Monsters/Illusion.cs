using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Illusion : Monster
{
    public override bool HasSoul => false;
    public override int Money => 0;

    public override async Task AfterAttack(Vector2Int delta) {
        await base.AfterAttack(delta);
        Debug.LogFormat($"Illusion {this} dies after attack");
        await Die();
    }

    protected override async Task BeforeDie() {
        await base.BeforeDie();
        if (lastAttacker != null && lastAttacker.GetComponent<Unit>() != null) {
            Debug.LogFormat($"{this} attacks killer {lastAttacker} before death");
            await Attack(lastAttacker.GetComponent<Unit>());
        }
    }

    protected override async Task AfterDie() {
        await base.AfterDie();
        var fog = figure.Location.GetFigure<Fog>();
        if (fog != null) {
            fog.On = true;
        }
    }
}
