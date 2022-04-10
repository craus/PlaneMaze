using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Common
{
    public class SetRandomPosition : Effect
    {
        public Transform target;
        public BoxCollider volume;

        public override void Run() {
            target.position = volume.transform.TransformPoint(Extensions.RandomPointInBounds(new Bounds(volume.center, volume.size)));
        }
    }
}