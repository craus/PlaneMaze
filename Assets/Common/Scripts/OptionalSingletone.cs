using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class OptionalSingletone<T> : MonoBehaviour where T : MonoBehaviour {
	private static T _instance;

	public static T instance {
		get {
			return _instance;
		}
	}

	public virtual void Awake() {
		_instance = this as T;
	}

	public virtual void OnDestroy() {
		Debug.LogFormat("OnDestroy {0}", this);
		if (_instance == this) {
			_instance = null;
		}
	}
}
