using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Terrain : MonoBehaviour, IScaring
{
    public virtual bool OccupiesTerrainPlace => true;
    public virtual bool Scaring(Unit unit) => false;

    public Figure figure;

    public virtual void Awake() {
        figure = GetComponent<Figure>();
    }
}
