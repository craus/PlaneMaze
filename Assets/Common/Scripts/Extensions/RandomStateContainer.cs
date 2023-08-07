using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomStateContainer
{
    public UnityEngine.Random.State state;

    public RandomStateContainer() {
        state = UnityEngine.Random.state;
    }
}