using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Common
{
    public class ConstantFloatProvider : FloatValueProvider
    {
        public float value;
        public override float Value => value;
    }
}