using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ListConvertProvider<T1, T2, IEnumerableProviderT1> : IEnumerableProvider<T2> 
    where IEnumerableProviderT1 : IEnumerableProvider<T1>
{
    public IEnumerableProviderT1 source;

    protected abstract T2 Convert(T1 from);

    public override IEnumerable<T2> Value => source.Value.Select(Convert);
}
