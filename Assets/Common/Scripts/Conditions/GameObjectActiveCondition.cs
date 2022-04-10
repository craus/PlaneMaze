using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class GameObjectActiveCondition : BoolValueProvider
    {
        public override bool Value => gameObject.activeInHierarchy;
    }
}
