using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Common
{
    public class FloatFormatter : StringValueProvider
    {
        [SerializeField] private FloatValueProvider timeProvider;
        [SerializeField] private string format = "F1";

        public override string Value => timeProvider.Value.ToString(format);
    }
}

