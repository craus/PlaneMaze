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
        var result = Instantiate(model.Sample);
        return result;
    }

    public abstract AscentionModel Save();
}
