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

        Player.instance.figure.afterMove.Add(AfterPlayerMove);
    }

    public void OnDestroy() {
        if (Player.instance != null) {
            Player.instance.figure.afterMove.Remove(AfterPlayerMove);
        }
    }

    private async Task AfterPlayerMove(Cell from, Cell to) {
        if (Mathf.Abs((to.position - GetComponent<Figure>().location.position).magnitude - 1) < 1e-4) {
            Show();
        }
    }

    public void Show() {
        sprite.SetActive(true);
    }
}
