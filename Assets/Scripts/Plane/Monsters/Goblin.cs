﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Goblin : Monster
{
    public Vector2Int currentDirection;
    public List<Transform> eyes;

    public override void Awake() {
        base.Awake();
        currentDirection = Moves.Rnd();
        UpdateSprite();
        new ValueTracker<Vector2Int>(() => currentDirection, v => {
            currentDirection = v; UpdateSprite();
        });
    }

    private void UpdateSprite() {
        eyes.ForEach(t => t.localPosition = (Vector2)currentDirection);
    }

    //сделай паттерн движения такой: 
    //пускай едет в каком-нибудь направлении, а когда не может, 
    //меняет направление на случайное из тех, 
    //в которые можно ехать.
    //чтобы она не билась в стену много раз.
    protected override async Task MakeMove() {
        if (!await SmartWalk(currentDirection)) {
            if (!await TryAttack(currentDirection)) {
                await SmartFakeMove(currentDirection);
                currentDirection = Moves.Rnd(m => figure.Location.Shift(m).Free);
                UpdateSprite();
            }
        }
    }
}
