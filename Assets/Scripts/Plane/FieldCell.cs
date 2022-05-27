using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FieldCell
{
    public bool wall;

    public bool teleport = false;
    public Vector2Int teleportTarget;

    public float difficulty = 0;

    public bool captured = false;
}
