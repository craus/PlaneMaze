using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Common;

public class GetBoolByMark : BoolValueProvider
{
    [Space]
    [SerializeField] private Mark mark;
    [SerializeField] private bool defaultValue;

    protected override bool BoolValue => mark.target ? mark.target.GetComponent<BoolValueProvider>().Value : defaultValue;
}
