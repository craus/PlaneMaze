using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public abstract class AbstractSubsetCondition<T> : BoolValueProvider
    {
        [SerializeField] protected List<T> set;
        [SerializeField] [ReadOnly] protected List<T> completed;

        public int requiredSubsetSize;

        [Space]

        [SerializeField] private UnityEvent onStepCompleted;
        public override bool Value => completed.Count >= requiredSubsetSize;

        protected virtual void Start()
        {
            completed = new List<T>();
        }

        protected void CompleteStep(T step)
        {
            completed.Add(step);
            onStepCompleted.Invoke();
            DebugManager.DebugValue("completed", "{0} of {1}".i(completed.Count, requiredSubsetSize));
            DebugManager.DebugValue(name, Value);
        }

        public void SequenceReset()
        {
            completed.Clear();
        }
    }
}
