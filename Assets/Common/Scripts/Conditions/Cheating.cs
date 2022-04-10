using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class Cheating : BoolValueProvider
    {
        public override bool Value => Cheats.on;
    }
}
