using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ConcatenationProvider<T, ListProviderT> : IEnumerableProvider<T> where ListProviderT : IEnumerableProvider<T>
{
    public List<ListProviderT> sources;

    public override IEnumerable<T> Value => sources.Aggregate(
        CollectionExtensions.Empty<T>(), 
        (a, b) => a.Concat(b.Value)
    );
}
