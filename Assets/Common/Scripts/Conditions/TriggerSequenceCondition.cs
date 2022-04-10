using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class TriggerSequenceCondition : AbstractSequenceCondition<Trigger>
    {
        protected override void Start()
        {
            base.Start();
            sequence.ForEach(t => t.activate.AddListener(() =>
            {
                if (completed.Count < sequence.Count)
                {
                    if (sequence[completed.Count] == t)
                    {
                        CompleteNextStep();
                    }
                }
            }));
        }
    }
}
