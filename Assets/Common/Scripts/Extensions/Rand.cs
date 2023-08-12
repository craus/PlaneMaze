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
	public static RandomStateContainer gameplay = new RandomStateContainer();
	public static RandomStateContainer visual = new RandomStateContainer();

	public static bool rndEvent(double p, RandomStateContainer source = null) {
		return Value(source) < p;
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
			result += Rand.Range(0, edges);
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
		return matrix[Rand.Range(0, matrix.GetLength(0)), Rand.Range(0, matrix.GetLength(1))];
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

	/// <summary>
	/// возвращает случайное подмножество коллекции заданного размера
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="collection"></param>
	/// <param name="cnt"></param>
	/// <returns></returns>
	public static IEnumerable<T> RndSelection<T>(this IEnumerable<T> collection, int cnt) {
		cnt = Mathf.Clamp(cnt, 0, collection.Count()); 
		int trash = collection.Count() - cnt;
		foreach (var x in collection) {
			if (Rand.Range(0, cnt + trash) < cnt) {
				yield return x;
				--cnt;
			} else {
				--trash;
			}
		};
	}

	public static List<T> RndSelection<T>(this List<T> collection, int cnt, IEnumerable<T> alwaysInclude) {
		return alwaysInclude.Concat(RndSelection(collection.Except(alwaysInclude).ToList(), cnt - alwaysInclude.Count())).ToList();
	}

	public static double Rnd(double min, double max) {
		return (double)(Rand.Range((float)min, (float)max));
	}

	public static float Rnd(float min, float max) {
		return Rand.Range(min, max);
	}

	public static float GaussianRnd() {
		float sum = 0;
		for (int i = 0; i < 12; i++) {
			sum += Rand.Range(-1f, 1f);
		}
		return sum;
	}

	public static T rnd<T>(this List<T> list) where T : class {
		if (list.Count == 0) return null;
		return list[Rand.Range(0, list.Count)];
	}

	public static int rnd(int min, int max) => Rand.Range(min, max+1);

	public static T rnd<T>(this List<T> list, Func<T, float> weight) where T : class {
		if (list.Count == 0) return null;
		float weightSum = list.Sum(weight);
		float resultWeight = Value() * weightSum;
		float currentWeight = 0;
		foreach (var x in list) {
			currentWeight += weight(x);
			if (currentWeight > resultWeight) {
				return x;
			}
		}
		throw new Exception("Failed to normalize weights!");
	}

	public static float Range(float min, float max) {
		return UnityEngine.Random.Range(min, max);
	}

	public static int Range(int min, int max) {
		return UnityEngine.Random.Range(min, max);
	}

	public static float GetRandomFloat(RandomStateContainer source, Func<float> f) {
		var oldState = UnityEngine.Random.state;
		UnityEngine.Random.state = source.state;

		var result = f();

		source.state = UnityEngine.Random.state;
		UnityEngine.Random.state = oldState;

		return result;
	}

	public static float Value(RandomStateContainer source = null) {
		if (source != null) {
			return GetRandomFloat(source, () => UnityEngine.Random.value);
		}
		return UnityEngine.Random.value;
	}

	public static T weightedRnd<T>(this List<Weighted<T>> list) where T : class {
		if (list.Count == 0) return null;
		float weightSum = list.Sum(x => x.weight);
		float resultWeight = Value() * weightSum;
		float currentWeight = 0;
		foreach (var x in list) {
			currentWeight += x.weight;
			if (currentWeight > resultWeight) {
				return x.to;
			}
		}
		throw new Exception("Failed to normalize weights!");
	}

	public static T rndExcept<T>(this List<T> list, IEnumerable<T> except) where T : class {
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