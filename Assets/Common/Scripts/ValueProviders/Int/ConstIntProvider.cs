using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConstIntProvider : IntValueProvider {
    public int value;
    public override int Value => value;
}
