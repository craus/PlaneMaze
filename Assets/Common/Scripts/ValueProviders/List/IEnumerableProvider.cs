using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class IEnumerableProvider<T> : ValueProvider<IEnumerable<T>>
{
    [ReadOnly]
    public List<T> listPreview;

    protected override void PreviewValueInternal()
    {
        base.PreviewValueInternal();
        listPreview = Value.ToList();
    }

}
