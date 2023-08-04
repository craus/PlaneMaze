using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ActivenessTracker : AbstractTracker<bool>
{
    protected override ValueTracker<bool> CreateTracker() {
        return new ValueTracker<bool>((v) => gameObject.SetActive(v), () => gameObject.activeSelf);
    }
}