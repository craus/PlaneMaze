using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public SpriteRenderer sprite;
    public GameObject hover;
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

    public void UpdateCell() {
        Debug.Log($"UpdateCell {name}");
        sprite.color = SpriteColor;
        hover.SetActive(Hovered);
    }

    public void Unhover() {
        UpdateCell();
    }

    public void Hover() {
        UpdateCell();
    }
}
