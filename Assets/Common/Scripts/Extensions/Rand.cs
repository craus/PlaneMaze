using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Rand
{
	public static bool rndEvent(double p) {
		return UnityEngine.Random.value < p;
	}

	public static int rndEvents(double p, int cnt) {
		int result = 0;
		for (int i = 0; i < cnt; i++) {
			if (rndEvent(p)) {
				++result;
			}
		}
		return result;
	}

	public static int d0(int cnt, int edges) {
		int result = 0;
		for (int i = 0; i < cnt; i++) {
			result += UnityEngine.Random.Range(0, edges);
		}
		return result;
	}

	public static int d(int cnt, int edges) {
		return d0(cnt, edges) + cnt;
	}

	public static int rndRound(this float x) {
		return rndEvent(x % 1) ? (int)Mathf.Ceil(x) : (int)Mathf.Floor(x);
	}

	public static T rnd<T>(T[,] matrix) {
		return matrix[UnityEngine.Random.Range(0, matrix.GetLength(0)), UnityEngine.Random.Range(0, matrix.GetLength(1))];
	}

	public static T rnd<T>(T[,] matrix, Func<T, bool> condition) {
		for (int i = 0; i < 100; i++) {
			var result = rnd(matrix);
			if (condition(result)) {
				return result;
			}
		}
		return rnd(matrix);
	}

	public static List<T> RndSelection<T>(this List<T> collection, int cnt) {
		if (cnt > collection.Count) {
			return collection;
		}
		List<T> result = new List<T>();
		int trash = collection.Count - cnt;
		collection.ForEach(x => {
			if (UnityEngine.Random.Range(0, cnt + trash) < cnt) {
				result.Add(x);
				--cnt;
			} else {
				--trash;
			}
		});
		return result;
	}

	public static List<T> RndSelection<T>(this List<T> collection, int cnt, IEnumerable<T> alwaysInclude) {
		return alwaysInclude.Concat(RndSelection(collection.Except(alwaysInclude).ToList(), cnt - alwaysInclude.Count())).ToList();
	}

	public static double Rnd(double min, double max) {
		return (double)(UnityEngine.Random.Range((float)min, (float)max));
	}

	public static float Rnd(float min, float max) {
		return UnityEngine.Random.Range(min, max);
	}

	public static float GaussianRnd() {
		float sum = 0;
		for (int i = 0; i < 12; i++) {
			sum += UnityEngine.Random.Range(-1f, 1f);
		}
		return sum;
	}

	public static T rnd<T>(this List<T> list) where T : class {
		if (list.Count == 0) return null;
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T rndExcept<T>(this List<T> list, ICollection<T> except) where T : class {
		return list.Except(except).ToList().rnd();
	}

	public static T rndExcept<T>(this List<T> list, params T[] except) where T : class {
		return list.Except(except).ToList().rnd();
	}

	public static float[] rndSplit(float total, int parts) {
		var borders = new List<float>();
		borders.Add(0);
		for (int i = 0; i < parts - 1; i++) {
			borders.Add(Rnd(0, total));
		}
		borders.Add(total);
		var partition = new float[parts];
		for (int i = 0; i < parts; i++) {
			partition[i] = borders[i + 1] - borders[i];
		}
		return partition;
	}
}