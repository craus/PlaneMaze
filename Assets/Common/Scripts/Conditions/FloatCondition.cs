using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class FloatCondition : BoolValueProvider
    {
        [SerializeField] private FloatValueProvider source;
        public float threshold;
        public bool less;

        public override bool Value => source.Value < threshold == less; 
    }
}
