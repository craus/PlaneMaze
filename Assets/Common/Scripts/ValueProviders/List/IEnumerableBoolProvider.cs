using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class IEnumerableBoolProvider : IEnumerableProvider<bool>
{
    [ContextMenu("Preview value")]
    public void PreviewValue()
    {
        PreviewValueCommand();
    }
}
