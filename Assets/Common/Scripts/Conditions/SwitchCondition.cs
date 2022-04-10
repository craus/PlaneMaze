using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class SwitchCondition : BoolValueProvider
    {
        [SerializeField] private bool on;
        public override bool Value => on;

        public void Switch(bool on)
        {
            this.on = on;
        }
    }
}
