using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class Ascention : MonoBehaviour
{
    public string abbreviation;

    public int maxLevel = 1;

    public static Ascention Load(AscentionModel model) {
        var result = (model.Sample); // no instantiate
        return result;
    }

    public abstract AscentionModel Save();

    public virtual IEnumerable<Ascention> Dependencies {
        get {
            yield break;
        }
    }

    public virtual bool CanAdd(Metagame metagame) {
        return metagame.ascentions.Count(x => x == this) < maxLevel && Dependencies.All(x => metagame.ascentions.Contains(x));
    }

    public virtual bool CanRemove(Metagame metagame) {
        return metagame.ascentions.All(m => !m.Dependencies.Contains(this));
    }
}
