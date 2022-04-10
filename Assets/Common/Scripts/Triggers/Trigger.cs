using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class Trigger : Effect
    {
        [SerializeField] private bool once;

        public UnityEvent activate;

        [ReadOnly] [SerializeField] private bool triggered;

        [ContextMenu("Run")]
        public override void Run()
        {
            if (!once || !triggered)
            {
                triggered = true;
                activate.Invoke();
            }
        }
    }
}
