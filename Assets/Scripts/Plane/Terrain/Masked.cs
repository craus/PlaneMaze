using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Masked : MonoBehaviour
{
    public int triggerVisibilityRadius = 4;

    public GameObject sprite;

    public void Trigger() {
        if ((Player.instance.figure.Location.position - GetComponent<Figure>().Location.position).MaxDelta() <= triggerVisibilityRadius) {
            Show();
        }
    }

    public void Awake() {
        Player.instance.figure.afterMove.Add(AfterPlayerMove);
        if (GetComponent<TeleportTrap>() == null && !Metagame.instance.Ascention<MaskedTerrain>()) {
            Show();
        } else {
            Hide();
        }

        new ValueTracker<bool>(
            () => sprite.gameObject.activeSelf,
            v => sprite.gameObject.SetActive(v),
            defaultValue: false
        );
    }

    public void OnDestroy() {
        if (Player.instance != null) {
            Player.instance.figure.afterMove.Remove(AfterPlayerMove);
        }
    }

    private async Task AfterPlayerMove(Cell from, Cell to) {
        if ((to.position - GetComponent<Figure>().Location.position).magnitude < 1 + 1e-4) {
            Show();
        }
    }

    public void Hide() {
        sprite.SetActive(false);
    }

    public void Show() {
        sprite.SetActive(true);
    }
}
