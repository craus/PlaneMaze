using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoolConstValueProvider : BoolValueProvider {
    public bool constant;
    public void SetValue(bool value)
    {
        constant = value;
    }
    public override bool Value => constant;
}
