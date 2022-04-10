using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class PriorityValueProvider<T> : ValueProvider<T> {
	public List<ValueSetter<T>> valueSetters = new List<ValueSetter<T>>();

	public T defaultValue;

	T currentValue;

	public override T Value {
		get {
			return currentValue;
		}
	}

	public void Start() {
		Recalculate();
	}

	public void AddValueSetter(ValueSetter<T> valueSetter) {
		valueSetters.Add(valueSetter);
		valueSetter.provider = this;
		OnChange();
	}

	public void RemoveValueSetter(ValueSetter<T> valueSetter) {
		valueSetters.Remove(valueSetter);
		OnChange();
	}

	public void Recalculate() {
		currentValue = defaultValue;
		var bestSetter = valueSetters.Where(v => v.Active).MaxBy(v => v.Priority);
		if (bestSetter != null) {
			currentValue = bestSetter.Value;
		}
		Changed();
	}

	public void OnChange() {
		Recalculate();
	}
}
