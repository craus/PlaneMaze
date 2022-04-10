using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class RigidBodyTracker : MonoBehaviour
{
    public new Rigidbody rigidbody;

    public bool keepWakeUp = false;
    //public bool trackWakeUp = true;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        //rigidbody.neve
    }

    public Vector3Tracker velocityTracker;
    public Vector3Tracker angularVelocityTracker;
    public BoolTracker isKinematicTracker;

    void Start() {
        velocityTracker = new Vector3Tracker((v) => rigidbody.velocity = v, () => rigidbody.velocity);
        angularVelocityTracker = new Vector3Tracker((v) => rigidbody.angularVelocity = v, () => rigidbody.angularVelocity);
        isKinematicTracker = new BoolTracker((v) => rigidbody.isKinematic = v, () => rigidbody.isKinematic);

//        new BoolTracker((v) => {
//            if (v) {
//                rigidbody.Sleep();
//            } else {
//                rigidbody.WakeUp();
//            }
//        }, () => rigidbody.IsSleeping());
    }

    void FixedUpdate() {
        if (keepWakeUp) {
            rigidbody.WakeUp();
        }
    }
}