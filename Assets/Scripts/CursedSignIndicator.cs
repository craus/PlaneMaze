using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CursedSignIndicator : Singletone<CursedSignIndicator>
{
    public Sprite sprite;

    public void Update() {
        Game.instance.GetComponent<CursedSignCounter>().cursedSignCount;
    }
}
