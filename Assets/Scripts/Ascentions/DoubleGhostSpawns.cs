using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class DoubleGhostSpawns : GenericAscention<DoubleGhostSpawnsModel>
{
    public override IEnumerable<Ascention> Dependencies => new List<Ascention> { Library.instance.doubleGhostSpawns };
}
