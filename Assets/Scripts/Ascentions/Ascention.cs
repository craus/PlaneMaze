using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class Ascention : MonoBehaviour
{
    public int maxLevel = 1;
    public List<Ascention> requirements;

    public static Ascention Load(AscentionModel model) {
        var result = (model.Sample); // no instantiate
        return result;
    }

    public abstract AscentionModel Save();

    public virtual bool CanAdd(Metagame metagame) {
        return true;
    }
}
