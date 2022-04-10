using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class DestroyGameObject : Effect
    {
        [SerializeField] private GameObject target;

        public override void Run()
        {
            Destroy(target);
        }
    }
}
