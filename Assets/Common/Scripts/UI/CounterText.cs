using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CounterText : MonoBehaviour {
	Text text;

	public AbstractCounter counter;

	public void Awake() {
		text = GetComponent<Text>();
	}

	public void Update() {
		text.text = counter.StringValue();
	}
}

