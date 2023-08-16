using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Weighted<Vertex> : IComparable
{
	public Vertex to;
	public float weight;
	public Weighted(Vertex to, float weight) {
		this.to = to;
		this.weight = weight;
	}

	public int CompareTo(object obj) {
		Weighted<Vertex> other = obj as Weighted<Vertex>;
		return weight.CompareTo(other.weight);
	}

	public override string ToString() {
		return $"{to} ({weight})";
	}
}