using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Coffin : Monster
{
    public override int Money => 0;
    public override bool HasSoul => false;
    public override bool Threatening => false;

    [SerializeField] private List<Weighted<Figure>> contentSamples;

    protected override async Task AfterDie() {
        await base.AfterDie();
        var childSample = contentSamples.weightedRnd();
        var p = GetComponent<Figure>().Location;
        if (childSample.gameObject == Library.instance.gem.gameObject) {
            Game.instance.AddGem(p, 1);
        } else {
            var child = Game.instance.GenerateFigure(p, childSample);
            if (child.GetComponent<MovesReserve>() != null) {
                await child.GetComponent<MovesReserve>().Freeze(1);
            }
        }
    }
}
