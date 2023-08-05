using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivenessTracker : MonoBehaviour
{
    public void Awake() {
        new ValueTracker<bool>(
            () => gameObject.activeSelf,
            v => gameObject.SetActive(v),
            defaultValue: false
        );
    }
}
