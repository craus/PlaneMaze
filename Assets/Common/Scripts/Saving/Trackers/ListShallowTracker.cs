using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class ListShallowTracker<T> : ValueTracker<List<T>>
{
    public ListShallowTracker(Action<List<T>> setList, Func<List<T>> getList)
        : base(
            setValue: (v) => setList(v.ShallowClone()),
            getValue: () => getList().ShallowClone(),
            isActual: (v) => v == getList() || getList() != null && v != null && getList().SequenceEqual(v)
        ) {
    }
}