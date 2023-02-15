using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class TeleportTrap : MonoBehaviour
{
    public int teleportRadius = 6;
    public int invulnerabilityTurns = 1;
    public int triggerVisibilityRadius = 4;

    public GameObject sprite;

    public void Awake() {
        GetComponent<Figure>().collide = async (from, figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null) {
                if ((Player.instance.figure.location.position - GetComponent<Figure>().location.position).MaxDelta() <= triggerVisibilityRadius) {
                    Show();
                }

                await victim.GetComponent<Invulnerability>().Gain(invulnerabilityTurns);
                await Helpers.TeleportAway(victim.figure, teleportRadius);
            }
        };
    }

    public void Show() {
        sprite.SetActive(true);
    }
}
