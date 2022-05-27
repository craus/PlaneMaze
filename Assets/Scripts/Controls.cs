using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Controls : Singletone<Controls>
{
    public Vector3 WorldCursor => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    [SerializeField] private Cell hovered;
    public Cell Hovered {
        get
        {
            return hovered;
        }
        set
        {
            var oldHovered = hovered;
            hovered = value;
            if (oldHovered != null) {
                oldHovered.Unhover();
            }
            if (hovered != null) {
                hovered.Hover();
            }
        }

    }

    public void Update() {
        Hovered = GameManager.instance.board.GetCellByPosition(WorldCursor);
    }
}
