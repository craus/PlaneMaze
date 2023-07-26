using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MetagameModel : Model
{
    public List<AscentionModel> ascentions;
    public bool pickingPhase = false;
    public int losesWithNoPenalty = 0;
    public bool runInProgress = false;
}
