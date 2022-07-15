﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Gem : MonoBehaviour
{
    public void OnDestroy() {
        Game.instance.gems.Remove(this);
    }
}
