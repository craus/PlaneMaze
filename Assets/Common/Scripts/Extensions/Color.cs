using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ExtColor
{
	public static Color withAlpha(this Color c, float alpha) {
		return new Color(c.r, c.g, c.b, alpha);
	}
}