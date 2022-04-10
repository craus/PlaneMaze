using RSG;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    public class TimerSpentTime : FloatValueProvider
    {
        [SerializeField] private Timer timer;

        public override float Value => timer.TimeSpent;
    }
}