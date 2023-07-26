using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Haste : Terrain
{
    public void Awake() {
        GetComponent<Figure>().collide = async (from, figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null) {
                if (victim.BenefitsFromTerrain) {
                    await victim.GetComponent<MovesReserve>().Haste(1);
                }
            }
        };
    }
}
