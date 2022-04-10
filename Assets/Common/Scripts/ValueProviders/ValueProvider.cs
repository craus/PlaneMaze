using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class ValueProvider<T> : AbstractValueProvider {
    public abstract T Value {
        get;
    }

    public override object ObjectValue => Value;

    public event Action onChange;

    public void Changed() {
        if (onChange != null) {
            onChange.Invoke();
        }
    }

    [SerializeField] [ReadOnly] private T valuePreview;
    [SerializeField] private bool enableValuePreview = false;

    protected virtual void PreviewValueCommand()
    {
        PreviewValueInternal();
        Debug.LogFormat($"Value: {Value}");
    }

    protected virtual void PreviewValueInternal()
    {
        valuePreview = Value;
    }

    protected virtual void Update() {
#if UNITY_EDITOR
        //if (enableValuePreview)
        {
            PreviewValueInternal();
        }
#endif
	}
}
