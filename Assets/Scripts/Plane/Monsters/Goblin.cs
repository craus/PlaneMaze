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

    public async Task<bool> TryAttack(Vector2Int delta) {
        if (figure.location.GetFigure<PeaceTrap>() != null) {
            return false;
        }

        var newPosition = figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Player>() != null)) {
            await Attack(newPosition.GetFigure<Player>());
            return true;
        }
        return false;
    }

    //сделай паттерн движения такой: 
    //пускай едет в каком-нибудь направлении, а когда не может, 
    //меняет направление на случайное из тех, 
    //в которые можно ехать.
    //чтобы она не билась в стену много раз.
    public override async Task Move() {
        if (!await figure.TryWalk(currentDirection)) {
            if (!await TryAttack(currentDirection)) {
                await figure.FakeMove(currentDirection);
                currentDirection = moves.Rnd(m => figure.location.Shift(m).Free);
            }
        }
    }
}
