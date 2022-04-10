using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ConstListProvider<T> : IEnumerableProvider<T>
{
    public List<T> constantValue;

    public override IEnumerable<T> Value => constantValue;
}
