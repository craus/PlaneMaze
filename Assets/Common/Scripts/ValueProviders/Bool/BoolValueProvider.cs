using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class BoolValueProvider : ValueProvider<bool>
{
    [Space]
    [SerializeField] private bool reversed;

    protected virtual bool BoolValue => false;

    public override bool Value => BoolValue != reversed;

    [ContextMenu("Preview value")]
    public void PreviewValue()
    {
        PreviewValueCommand();
    }
}
