using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Portal : Terrain
{
    public bool activated = false;

    public Portal second;

    public override void Awake() {
        base.Awake();
        GetComponent<Figure>().collide = async (figure) => {
            if (second.activated) {
                return;
            }
            var victim = figure.GetComponent<Player>();
            if (victim != null) {
                activated = true;
                SoundManager.instance.teleport.Play();
                await victim.figure.Move(second.GetComponent<Figure>().Location, isTeleport: true);
                await victim.GetComponent<MovesReserve>().Haste(1);

                activated = false;
            }
        };
    }
}
