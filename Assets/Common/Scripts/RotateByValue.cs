using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class RotateByValue : MonoBehaviour {
	public FloatValueProvider angleProvider;

    public Vector3 axis;

	void Update() {
        transform.localRotation = Quaternion.Euler(axis * angleProvider.Value);
	}
}
