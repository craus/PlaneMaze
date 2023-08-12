using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class ShallowGrave : Terrain
{
    public override void Awake() {
        base.Awake();
        GetComponent<Figure>().collide = async (from, figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null && !victim.Flying && victim.HasSoul) {
                if (victim.GetComponent<Player>()) {
                    SoundManager.instance.shallowGrave.Play();
                }
                await victim.GetComponent<Root>().Gain(2);
                GetComponent<Masked>().Trigger();
            }
        };
    }
}
