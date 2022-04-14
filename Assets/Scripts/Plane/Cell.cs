using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Color wallColor;
    public Color emptyColor;

    public Vector2Int position;

    public FieldCell fieldCell;
    public Board board;

    public HashSet<Figure> figures = new HashSet<Figure>();

    public void SetFieldCell(FieldCell fieldCell) {
        this.fieldCell = fieldCell;
        sprite.color = fieldCell.wall ? wallColor : emptyColor;
        //sprite.color = fieldCell.color;
    }

    public Cell Shift(Vector2Int delta) {
        return board.GetCell(position + delta);
    }
}
