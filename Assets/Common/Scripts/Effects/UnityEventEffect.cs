using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class UnityEventEffect : Effect
    {
        [SerializeField] private UnityEvent onRun;

        public override void Run()
        {
            onRun.Invoke();
        }
    }
}
