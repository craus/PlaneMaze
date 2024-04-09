using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class TeleportTrap : Terrain
{
    public int teleportRadius = 6;
    public int invulnerabilityTurns = 1;

    public override void Awake() {
        base.Awake();
        GetComponent<Figure>().collide = async (figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null) {
                GetComponent<Masked>().Trigger();

                await victim.GetComponent<Invulnerability>().Gain(invulnerabilityTurns);
                await Helpers.TeleportAway(victim.figure, teleportRadius);
            }
        };
    }
}
