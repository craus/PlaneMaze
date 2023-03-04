using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Walk : MoveAction
{
    public Cell from;
    public Cell to;

    public Walk(Cell from, Cell to) {
        this.from = from;
        this.to = to;
    }
}
