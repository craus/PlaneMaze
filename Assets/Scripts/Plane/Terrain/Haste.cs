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
                if (victim.GetComponent<Player>()) {
                    SoundManager.instance.additionalMove.timeSamples = SoundManager.instance.additionalMove.clip.samples - 1;
                    SoundManager.instance.additionalMove.Play();
                }
                await victim.GetComponent<MovesReserve>().Haste(1);
            }
        };
    }
}
