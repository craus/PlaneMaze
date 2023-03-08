using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SkeletonsResurrectModel : AscentionModel
{
    public override Ascention Sample => Library.instance.doubleGhostSpawns;
}
