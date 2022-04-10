using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Common
{
    public class SetRandomRotation : Effect
    {
        public Transform target;

        public override void Run() {
            target.rotation = Extensions.GenerateWithCondition(
                () => {
                    Quaternion q = Quaternion.identity;
                    q.SetLookRotation(Random.onUnitSphere, Random.onUnitSphere);
                    return q;
                }, 
                q => (q * Vector3.up).y > 0.2f
            );
        }
    }
}