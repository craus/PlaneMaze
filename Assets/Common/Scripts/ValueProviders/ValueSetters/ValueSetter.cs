using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ValueSetter<T> : MonoBehaviour {
	[SerializeField]
	T value;

	public PriorityValueProvider<T> provider;

	public T Value {
		get {
			return value;
		}
		set {
			this.value = value;
			OnChange();
		}
	}

	[SerializeField]
	float priority;

	public float Priority {
		get {
			return priority;
		}
		set {
			priority = value;
			OnChange();
		}
	}

	[SerializeField]
	bool active;

	public bool Active {
		get {
			return active;
		}
		set {
			active = value;
			OnChange();
		}
	}

	void OnChange() {
		if (provider == null) {
			return;
		}
		provider.OnChange();
	}
}
