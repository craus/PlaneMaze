﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Club : Weapon
{
    public override async Task<bool> TryAttack(Vector2Int delta) {
        var newPosition = Owner.figure.location.Shift(delta);
        var target = newPosition.GetFigure<Unit>(u => u.Vulnerable);
        if (target == null) {
            return false;
        }
        if (!target.figure.location.Shift(delta).Free) {
            return false;
        } 
        if (!await Attack(target)) {
            return false;
        }
        if (target.alive && target.Movable) {
            await target.figure.TryWalk(delta);
        }
        return true;
    }

    public override Task<bool> AfterFailedWalk(Vector2Int delta) => TryAttack(delta);
}
