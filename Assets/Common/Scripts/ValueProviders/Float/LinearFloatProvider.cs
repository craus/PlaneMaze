using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Endo
{
    public class LinearFloatProvider : FloatValueProvider
    {
        [SerializeField] private FloatValueProvider source;

        public float multiplier = 1;
        public float addendum = 0;
        public override float Value => multiplier * source.Value + addendum;
    }
}