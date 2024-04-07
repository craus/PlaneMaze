using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Hellhound : Monster
{
    public override bool FireImmune => true;

    private void TryFire(Cell location) {
        if (location.GetFigure<Terrain>() != null) return;
        Game.GenerateFigure(location, Library.instance.fire);
    }

    protected override async Task MakeMove() {

        Vector2Int playerDelta = Player.instance.figure.Location.position - figure.Location.position;

        if (playerDelta.SumDelta() == 1) {
            if (await TryAttack(playerDelta)) {
                TryFire(figure.Location.Shift(playerDelta));
                return;
            }
        }

        int successfulMoves = 0;
        Cell beforeSecondMove = null;

        for (int i = 0; i < 2; i++) {
            var delta = Helpers.Moves.Rnd();
            GetComponent<SpriteDirection>().SetDirection(delta);
            if (i == 1) {
                beforeSecondMove = figure.Location;
            }
            if (await SmartWalk(delta)) {
                successfulMoves++;
                if (successfulMoves == 2 && beforeSecondMove.figures.Count == 0) {
                    TryFire(beforeSecondMove);
                }
            } else {
                await figure.FakeMove(delta);
            }
        }
    }
}
