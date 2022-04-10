using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Common;

public class FireRandomEvent : Effect {
	public List<UnityEvent> events;

	public override void Run() {
		events.rnd().Invoke();
	}
}
