using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Common
{
    public class TimeFormatter : StringValueProvider
    {
        [SerializeField] private FloatValueProvider timeProvider;
        [SerializeField] private string format = @"mm\:ss";

        public override string Value => TimeSpan.FromSeconds(timeProvider.Value+1).ToString(format);
    }
}

