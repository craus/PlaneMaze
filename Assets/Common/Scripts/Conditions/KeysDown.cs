using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

namespace Common
{
    public class KeysDown : BoolValueProvider
    {
        [SerializeField] private List<KeyCode> keys;

        public override bool Value => keys.All(key => Input.GetKey(key));
    }
}
