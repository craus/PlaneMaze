using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActivateOnCondition : MonoBehaviour {
	public BoolValueProvider condition;

	public List<GameObject> targets;

	public void Update() {
		targets.ForEach(t => t.SetActive(condition.Value));
	}
}
