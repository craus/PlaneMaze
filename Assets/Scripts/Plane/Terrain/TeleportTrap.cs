using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class TeleportTrap : MonoBehaviour
{
    public void Awake() {
        GetComponent<Figure>().collide = async (from, figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null) {
                var destination = GetComponent<Figure>().location.Vicinity(maxDx: 2, maxDy: 2).Where(c => c.Free).Rnd();
                await victim.GetComponent<Figure>().Move(destination, isTeleport: true);
            }
        };
    }
}
