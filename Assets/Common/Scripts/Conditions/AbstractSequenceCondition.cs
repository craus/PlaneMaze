using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public abstract class AbstractSequenceCondition<T> : BoolValueProvider
    {
        [SerializeField] protected List<T> sequence;
        [SerializeField] [ReadOnly] protected List<T> completed;
        public T Current => completed.Count == sequence.Count ? default : sequence[completed.Count];

        [Space]

        [SerializeField] private UnityEvent onStepCompleted;
        public override bool Value => sequence.Count == completed.Count;

        protected virtual void Start()
        {
            completed = new List<T>();
        }

        protected void CompleteNextStep()
        {
            completed.Add(sequence[completed.Count]);
            onStepCompleted.Invoke();
            DebugManager.DebugValue("completed", "{0} of {1}".i(completed.Count, sequence.Count));
            DebugManager.DebugValue(name, Value);
        }

        public void SequenceReset()
        {
            completed.Clear();
        }
    }
}
