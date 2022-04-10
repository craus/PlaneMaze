using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class TriggerSubsetCondition : AbstractSubsetCondition<Trigger>
    {
        protected override void Start()
        {
            base.Start();
            set.ForEach(t => t.activate.AddListener(() =>
            {
                if (!completed.Contains(t))
                {
                    CompleteStep(t);
                }
            }));
        }
    }
}
