using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using RSG;
using UnityEngine.Events;
using Common;

public class Undestroyable : Effect
{
	public UnityEvent effect;
	public bool destroyLater = true;

	public float delay = 10;

	public override void Run() {
		transform.SetParent(null);
		effect.Invoke();
		if (destroyLater) {
			TimeManager.Wait(delay).Then(() => {
				if (this != null) {
					Destroy(gameObject);
				}
			}).Done();
		}
	}
}
