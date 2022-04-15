using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Player : MonoBehaviour
{
    public Figure figure;
    public int gems;

    public void Awake() {
        if (figure == null) figure = GetComponent<Figure>();
        figure.afterMove.AddListener(AfterMove);
    }

    public void AfterMove(bool isTeleport) {
        if (!isTeleport) {
            if (figure.location.fieldCell.teleport) {
                figure.Move(figure.location.board.GetCell(figure.location.fieldCell.teleportTarget), isTeleport: true);
            }
        }

        figure.location.figures.Select(f => f.GetComponent<Gem>()).Where(g => g != null).ForEach(Take);
    }

    public void Take(Gem gem) {
        Destroy(gem.gameObject);
        gems++;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            figure.TryMove(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            figure.TryMove(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            figure.TryMove(Vector2Int.right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            figure.TryMove(Vector2Int.left);
        }
    }
}
