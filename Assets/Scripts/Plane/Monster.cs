using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Monster : Unit
{
    private List<Vector2Int> moves = new List<Vector2Int>() {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
    };

    public void Move() {
        figure.TryMove(moves.Rnd());
    }
}
