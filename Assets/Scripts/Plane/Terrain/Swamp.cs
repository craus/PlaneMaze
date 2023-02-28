using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Swamp : MonoBehaviour
{
    public void Awake() {
        GetComponent<Figure>().collide = async (from, figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null && !victim.Flying) {
                if (victim.GetComponent<Player>()) {
                    SoundManager.instance.swampDebuff.Play();
                }
                await victim.GetComponent<MovesReserve>().Freeze(1);
            }
        };
    }
}
