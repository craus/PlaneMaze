﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Spear : Weapon
{
    public override Task<bool> BeforeWalk(Vector2Int delta) => TryAttack(delta);

    public override IEnumerable<Vector2Int> AttackVectors() {
        yield return new Vector2Int(2, 0);
    }

    public override async Task<bool> TryAttack(Vector2Int delta) {
        var currentPosition = Owner.figure.location;
        var target = Helpers.RayCast(Owner.figure.location, delta, target: c => c.GetFigure<Monster>(m => m.Vulnerable) != null, distance: 2);
        if (target == null) {
            return false;
        }
        var victim = target.GetFigure<Monster>(m => m.Vulnerable);
        if (victim != null && (victim.figure.location.position - Owner.figure.location.position).sqrMagnitude == 4 && await Attack(victim)) {
            await Owner.GetComponent<MovesReserve>().Haste(1);
            await Owner.GetComponent<Disarm>().Gain(2);
            return true;
        }
        return false;
    }
}