using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Phylactery : Monster
{
    public override bool HasSoul => false;
    public override int Money => 0;
}
