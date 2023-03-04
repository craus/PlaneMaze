using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class FailedMove : MoveAction
{
    public Vector2Int direction;

    public FailedMove(Vector2Int direction) {
        this.direction = direction;
    }
}
