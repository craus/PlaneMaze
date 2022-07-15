using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Player : MonoBehaviour
{
    public Figure figure;
    public int totalGems;
    public int gems;

    public int wallCost = 2;

    public Wall wallSample;

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

        figure.location.figures.Select(f => f.GetComponent<Gem>()).Where(g => g != null).ToList().ForEach(Take);

        Game.instance.AfterPlayerMove();
    }

    public void Take(Gem gem) {
        Destroy(gem.gameObject);
        totalGems++;
        gems++;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            figure.TryMove(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            figure.TryMove(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            figure.TryMove(Vector2Int.right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            figure.TryMove(Vector2Int.left);
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            BuildWall(figure.location);
        }
    }

    public void BuildWall(Cell place) {
        if (gems < wallCost) {
            return;
        }
        if (place.figures.Any(p => p.GetComponent<Building>())) {
            return;
        }
        gems -= wallCost;
        Instantiate(wallSample, Game.instance.figureParent).GetComponent<Figure>().Move(place);
    }
}
