using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class SequenceCondition : AbstractSequenceCondition<BoolValueProvider>
    {
        protected override void Update()
        {
            if (sequence.Count > completed.Count && sequence[completed.Count].Value)
            {
                CompleteNextStep();
            }
        }
    }
}
