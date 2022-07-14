using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FieldCell
{
    public bool wall;
    public bool trap;

    public bool teleport = false;
    public Vector2Int teleportTarget;
}
