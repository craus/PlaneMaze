using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class DurationCondition : BoolValueProvider
    {
        [SerializeField] private BoolValueProvider condition;
        [SerializeField] private float duration;

        [SerializeField] [ReadOnly] private float lastFalseTime;
        [SerializeField] [ReadOnly] private bool inited = false;

        public override bool Value {
            get
            {
                if (!inited)
                {
                    Update();
                }
                return lastFalseTime < TimeManager.Time() - duration;
            }
        }

        protected override void Update()
        {
            inited = true;
            if (!condition.Value)
            {
                lastFalseTime = TimeManager.Time();
            }
        }
    }
}
