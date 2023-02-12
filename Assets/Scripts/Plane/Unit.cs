using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Unit : MonoBehaviour
{
    public Figure figure;

    public virtual void Awake() {
        if (figure == null) figure = GetComponent<Figure>();
        figure.afterMove.AddListener(AfterMove);
    }

    public virtual async void AfterMove(Cell from, bool isTeleport) {
        if (!isTeleport) {
            if (figure.location.fieldCell.teleport) {
                await figure.Move(figure.location.board.GetCell(figure.location.fieldCell.teleportTarget), isTeleport: true);
            }
        }
    }

    public virtual async Task Hit(int damage) {
        if (this == null) {
            return;
        }
        GetComponent<Health>().Current -= damage;
    }

    public virtual void Die() {
        Destroy(gameObject);
    }
}
