using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class LavaLake : Fire
{
    protected override void OnLeave() {
        // do nothing
    }
}
