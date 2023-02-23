using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Portal : MonoBehaviour
{
    public Portal second;

    public void Awake() {
        GetComponent<Figure>().collide = async (from, figure) => {
            if (from == second.GetComponent<Figure>().location) {
                return;
            }
            var victim = figure.GetComponent<Player>();
            if (victim != null) {
                await victim.figure.Move(second.GetComponent<Figure>().location, isTeleport: true);
            }
        };
    }
}
