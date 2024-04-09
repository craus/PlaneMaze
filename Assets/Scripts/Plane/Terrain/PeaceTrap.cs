using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class PeaceTrap : Terrain
{
    public override void Awake() {
        base.Awake();
        GetComponent<Figure>().collide = async (figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null) {
                if (victim.GetComponent<Player>()) {
                    SoundManager.instance.peaceDebuff.Play();
                }
            }
        };
    }
}
