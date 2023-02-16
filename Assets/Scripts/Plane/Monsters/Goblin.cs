using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Goblin : Monster
{
    public Vector2Int currentDirection;

    public override void Awake() {
        base.Awake();
        currentDirection = moves.Rnd();
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
                currentDirection = moves.Rnd(m => figure.location.Shift(m).Free);
            }
        }
    }
}
