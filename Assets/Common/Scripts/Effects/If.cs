using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class If : Effect
    {
        [SerializeField] private UnityEvent trigger;
        [SerializeField] private BoolValueProvider condition;

        public override void Run()
        {
            if (condition.Value) trigger.Invoke();
        }
    }
}
