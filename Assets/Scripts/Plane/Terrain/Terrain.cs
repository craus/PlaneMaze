using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public virtual bool OccupiesTerrainPlace => true;
    public Figure figure;

    public virtual void Awake() {
        figure = GetComponent<Figure>();
    }
}
