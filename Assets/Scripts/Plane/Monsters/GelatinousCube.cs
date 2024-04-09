using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GelatinousCube : Monster
{
    public override int Money => 0;
    public override bool HasSoul => false;
    public override bool Threatening => false;
    public override bool PoisonImmune => true;

    public override void Awake() {
        base.Awake();

        //figure.afterMove.Add(async (from, to) => {
        //    GelatinousCubesManager.instance.AfterGelatinousCubeMoved(from, to);
        //});
    }

    private async Task GenerateCube(Cell cell) {
        await Helpers.Delay(0.1f);
        if (!cell.Wall && cell.figures.Count == 0) {
            if (Rand.rndEvent(0.2f)) {
                Game.GenerateFigure(cell, Library.instance.gelatinousCube);
            }
        }
    }

    protected override async Task MakeMove() {
        List<Task> generateCubes = new List<Task>();
        figure.Location.Vicinity(1).ForEach(cell => {
            if (!cell.Wall && cell.figures.Count == 0) {
                if (cell.Vicinity(1).SelectMany(cell => cell.GetFigures<GelatinousCube>()).Count() >= 3) {
                    generateCubes.Add(GenerateCube(cell)); 
                }
            }
        });

        await Task.WhenAll(generateCubes);
    }
}
