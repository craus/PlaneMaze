using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class ConditionalFloatValueProvider : FloatValueProvider
    {
        public BoolValueProvider condition;

        public FloatValueProvider whenTrue;
        public FloatValueProvider whenFalse;

        public override float Value => condition.Value ? whenTrue.Value : whenFalse.Value;
    }
}