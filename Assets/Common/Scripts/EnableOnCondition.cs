using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnableOnCondition : MonoBehaviour {
	[SerializeField] private Behaviour target;
	[SerializeField] private BoolValueProvider condition;

	public void Update()
	{
		target.enabled = condition.Value;
	}
}
