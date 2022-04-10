using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Counter : IntValueProvider {
	[SerializeField] private int value;

	public override int Value => value;

	public int minValue = int.MinValue;

	public void Increment() {
		value += 1;
	}

	public void Decrement() {
		value -= 1;
		if (value < minValue) {
			value = minValue;
		}
	}

	public void SetValue(int value) {
		this.value = value;
	}

	public void Reset() {
		value = 0;
	}
}
