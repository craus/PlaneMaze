using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AcceleratingGhostSpawns : GenericAscention<AcceleratingGhostSpawnsModel>
{
    public override bool CanAdd(Metagame metagame) {
        return base.CanAdd(metagame) && metagame.SpawnGhosts;
    }
}
