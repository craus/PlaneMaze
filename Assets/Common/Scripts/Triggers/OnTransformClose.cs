using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class OnTransformClose : Trigger
    {
        [SerializeField] private Transform a;
        [SerializeField] private Transform b;
        [SerializeField] private float distance = 0.03f;
        [SerializeField] private float angleDistance = 15;

        public void Update() {
            if (Vector3.Distance(a.position, b.position) < distance && Quaternion.Angle(a.rotation, b.rotation) < angleDistance) {
                Run();
            }
        }
    }
}
