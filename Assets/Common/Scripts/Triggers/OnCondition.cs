using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class OnCondition : Trigger
    {
        [SerializeField] private BoolValueProvider condition;

        [SerializeField] private bool onValueChangedOnly = false;

        private void Start()
        {
            if (condition == null) condition = GetComponent<BoolValueProvider>();
        }

        private bool oldValue;

        private void Update()
        {
            if ((!onValueChangedOnly || !oldValue) && condition.Value)
            {
                Run();
            }
            oldValue = condition.Value;
        }
    }
}
