using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public SpriteRenderer sprite;
    public GameObject hover;
    public GameObject captured;
    public TMPro.TextMeshPro text;
    public Color wallColor;
    public Color emptyColor;
    public Color hoveredColor;
    public Color hoveredWallColor;

    public Vector2Int position;

    public FieldCell fieldCell;
    public Board board;

    public HashSet<Figure> figures = new HashSet<Figure>();

    public void SetFieldCell(FieldCell fieldCell) {
        this.fieldCell = fieldCell;
        UpdateCell();
        //sprite.color = fieldCell.color;
    }

    public Cell Shift(Vector2Int delta) {
        return board.GetCell(position + delta);
    }

    public Color SpriteColor 
    {
        get
        {
            return fieldCell.wall ? wallColor : emptyColor;
        }
    }

    public bool Hovered => Controls.instance.Hovered == this;

    public bool Captured => fieldCell.captured;

    public IEnumerable<Cell> Neighbours
    {
        get
        {
            yield return Shift(Vector2Int.up);
            yield return Shift(Vector2Int.right);
            yield return Shift(Vector2Int.down);
            yield return Shift(Vector2Int.left);
        }
    }

    public bool Capturable => !Captured && !fieldCell.wall && Neighbours.Any(c => c.Captured);

    public void UpdateCell() {
        sprite.color = SpriteColor;
        hover.SetActive(Hovered);
        text.text = $"{Cost.Digits(2)}";
        captured.SetActive(fieldCell.captured);
        text.gameObject.SetActive(!fieldCell.captured && !fieldCell.wall);
    }

    public void Unhover() {
        UpdateCell();
    }

    public void Hover() {
        UpdateCell();
    }

    public float Cost => fieldCell.difficulty / GameManager.instance.game.area * GameManager.instance.game.border;

    public void Capture(bool forced = false) {
        if (!forced) {
            if (fieldCell.wall) {
                return;
            }
            if (!Capturable) {
                return;
            }
        }
        if (Captured) {
            return;
        }
        fieldCell.captured = true;
        if (!forced) {
            GameManager.instance.game.time += Cost;
        }
        GameManager.instance.game.area++;
        GameManager.instance.game.border += Neighbours.Count(c => c.Capturable) - Neighbours.Count(c => c.Captured);
        GameManager.instance.board.UpdateAllCells();

        UpdateCell();
    }
}
