using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class CursedSign : Terrain, IInvisibilitySource, IOnDestroyHandler, IAttacker
{
    public bool Invisible => (Player.instance.figure.Location.position - GetComponent<Figure>().Location.position).MaxDelta() > 2;

    public override void Awake() {
        base.Awake();

        figure.collide = async (from, figure) => {
            if (figure == null) {
                return;
            }
            var victim = figure.GetComponent<Player>();
            if (victim != null) {
                await victim.GetComponent<Root>().Gain(2);
                if (this == null) {
                    return;
                }
                this.SoftDestroy(gameObject);
            }
        };
    }

    public void OnSoftDestroy() {
        Game.instance.GetComponent<CursedSignCounter>().cursedSignCount--;
    }
}
