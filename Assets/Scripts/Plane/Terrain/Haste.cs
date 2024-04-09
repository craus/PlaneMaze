using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Haste : Terrain
{
    public override void Awake() {
        base.Awake();
        GetComponent<Figure>().collide = async (figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null) {
                if (victim.BenefitsFromTerrain) {
                    await victim.GetComponent<MovesReserve>().Haste(1);
                    GetComponent<Masked>().Trigger();
                }
            }
        };
    }
}
