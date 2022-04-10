using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActivateRandomOnCondition : MonoBehaviour {
	public BoolValueProvider condition;

	public bool activated = false;

	public List<GameObject> targets;

	public void Activate() {
		activated = true;
		targets.rnd().SetActive(true);
	}

	public void Deactivate() {
		activated = false;
		targets.ForEach(t => t.SetActive(false));
	}

	public void Start() {
		Deactivate();
		if (condition.Value) {
			Activate();
		} 
	}

	public void Update() {
		if (condition.Value && !activated) {
			Activate();
		}
		if (!condition.Value && activated) {
			Deactivate();
		}
	}
}
