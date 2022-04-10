using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Algorithm
{
	public static float BinarySearch(float min, float max, Func<float, bool> bigEnough) {
		var result = min;
		var step = max - min;
		while (step > float.Epsilon) {
			if (!bigEnough(result + step)) {
				result += step;
			}
			step /= 2;
		}
		return result;
	}

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
	}

	public static Map<Vertex, Weighted<Vertex>> Dijkstra<Vertex>(Vertex start, Func<Vertex, IEnumerable<Weighted<Vertex>>> edges, int maxSteps = 10000) {
		Map<Vertex, Weighted<Vertex>> result = new Map<Vertex, Weighted<Vertex>>(() => new Weighted<Vertex>(default(Vertex), float.PositiveInfinity));
		HashSet<Vertex> relaxated = new HashSet<Vertex>();
		result[start] = new Weighted<Vertex>(default(Vertex), 0);
		for (int i = 0; i < maxSteps; i++) {
			var vertex = result.Keys.Except(relaxated).MinBy(v => result[v].weight);
			if (vertex == null) {
				break;
			}
			relaxated.Add(vertex);
			edges(vertex).ForEach(e => {
				var newPath = result[vertex].weight + e.weight;
				if (newPath < result[e.to].weight) {
					result[e.to] = new Weighted<Vertex>(vertex, newPath);
				}
			});
		}
		return result;
	}

	public static void Prim<Vertex>(Vertex start, Func<Vertex, IEnumerable<Weighted<Vertex>>> edges, Func<Vertex, bool> terminalVertex = null, Action<Vertex> visit = null) {
		terminalVertex ??= v => false;
		visit ??= v => { };

		PriorityQueue<Weighted<Vertex>> candidates = new PriorityQueue<Weighted<Vertex>>();
		HashSet<Vertex> visited = new HashSet<Vertex>();

		visited.Add(start);
		edges(start).ForEach(candidates.Enqueue);
		visit(start);

		while (candidates.Count > 0) {
			var current = candidates.Dequeue();
			if (visited.Contains(current.to)) {
				continue;
			}

			visited.Add(current.to);
			edges(current.to).ForEach(candidates.Enqueue);
			visit(current.to);

			if (terminalVertex(current.to)) {
				return;
			}
		}
	}
}