using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class GenericAscention<TAscentionModel> : Ascention where TAscentionModel : AscentionModel, new()
{
    public override AscentionModel Save() => new TAscentionModel();
}
