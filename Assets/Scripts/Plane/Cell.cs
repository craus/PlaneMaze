using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Color wallColor;
    public Color emptyColor;

    public void SetFieldCell(FieldCell fieldCell) {
        sprite.color = fieldCell.wall ? wallColor : emptyColor;
    }
}
