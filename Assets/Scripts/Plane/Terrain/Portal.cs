using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Portal : Terrain
{
    public Portal second;

    public override void Awake() {
        base.Awake();
        GetComponent<Figure>().collide = async (from, figure) => {
            if (from == second.GetComponent<Figure>().Location) {
                return;
            }
            var victim = figure.GetComponent<Player>();
            if (victim != null) {
                SoundManager.instance.teleport.Play();
                await victim.figure.Move(second.GetComponent<Figure>().Location, isTeleport: true);
                await victim.GetComponent<MovesReserve>().Haste(1);
            }
        };
    }
}
