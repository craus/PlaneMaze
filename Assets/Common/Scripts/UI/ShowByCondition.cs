using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShowByCondition : MonoBehaviour {
	GameObject content;

	public BoolValueProvider condition;

	public void Awake() {
		content = transform.Children()[0].gameObject;
	}

	public void Update() {
		content.SetActive(condition.Value);
	}
}
