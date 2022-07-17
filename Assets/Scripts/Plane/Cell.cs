using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Color wallColor;
    public Color emptyColor;
    public Color lockedColor;
    public Color darknessColor;

    public Vector2Int position;
    public int order;

    public FieldCell fieldCell;
    public Board board;

    [SerializeField] private bool dark = true;
    public bool Dark
    {
        get => dark;
        set
        {
            dark = value;
            Game.instance.OnCellDarknessChanged(this);
            UpdateCell();
        }
    }

    public bool Wall
    {
        get => fieldCell.wall;
        set {
            fieldCell.wall = value;
            UpdateCell();
        }
    }

    public bool Locked => order > Game.instance.unlockedCells;

    public HashSet<Figure> figures = new HashSet<Figure>();

    public Color Color() {
        if (Wall) return wallColor;
        if (Locked) return lockedColor;
        if (dark) return darknessColor;
        return emptyColor;
    }

    public void UpdateCell() {
        sprite.color = Color();
    }

    public void SetFieldCell(FieldCell fieldCell) {
        this.fieldCell = fieldCell;
        UpdateCell();
    }

    public Cell Shift(Vector2Int delta) {
        return board.GetCell(position + delta);
    }

    public IEnumerable<Cell> Neighbours() {
        yield return Shift(Vector2Int.up);
        yield return Shift(Vector2Int.down);
        yield return Shift(Vector2Int.left);
        yield return Shift(Vector2Int.right);
    }

    public T GetFigure<T>() => figures.Select(f => f.GetComponent<T>()).FirstOrDefault(t => t != null);
}
