using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Tree : Monster
{
    public override int Money => 0;
    public override bool HasSoul => false;
    public override bool Movable => false;
}
