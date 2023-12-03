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

	public static IEnumerable<Vertex> Prim<Vertex>(
		Vertex start, 
		Func<Vertex, IEnumerable<Weighted<Vertex>>> edges, 
		Func<Vertex, bool> terminalVertex = null,
		int maxSteps = (int)1e9,
		Action<Vertex> visit = null
	) {
		terminalVertex ??= v => false;
		visit ??= v => { };

		PriorityQueue<Weighted<Vertex>> candidates = new PriorityQueue<Weighted<Vertex>>();
		HashSet<Vertex> visited = new HashSet<Vertex>();
		var steps = 0;

		visited.Add(start);
		yield return start;
		edges(start).ForEach(candidates.Enqueue);
		visit(start);
		steps++; 
		if (steps == maxSteps) {
			yield break;
		}

		while (candidates.Count > 0) {
			var current = candidates.Dequeue();
			if (visited.Contains(current.to)) {
				continue;
			}

			visited.Add(current.to);
			yield return current.to;
			edges(current.to).ForEach(candidates.Enqueue);
			visit(current.to);
			steps++;
			if (steps == maxSteps) {
				yield break;
			}

			if (terminalVertex(current.to)) {
				yield break;
			}
		}
	}

	public static IEnumerable<Vertex> PrimDynamic<Vertex>(
		Vertex start,
		Func<Vertex, IEnumerable<Weighted<Vertex>>> edges,
		Func<Vertex, IEnumerable<Vertex>> antiEdges = null,
		Func<Vertex, bool> terminalVertex = null,
		int maxSteps = (int)1e9,
		Action<Vertex> visit = null
	) {
		terminalVertex ??= v => false;
		visit ??= v => { };
		antiEdges ??= v => CollectionExtensions.Empty<Vertex>();

		SortedSet<Weighted<Vertex>> candidates = new SortedSet<Weighted<Vertex>>();
		Map<Vertex, List<Weighted<Vertex>>> vertexCandidates = new Map<Vertex, List<Weighted<Vertex>>>(() => new List<Weighted<Vertex>>());
		HashSet<Vertex> visited = new HashSet<Vertex>();
		var steps = 0;

		Debug.LogFormat($"Start from: {start}");
		visited.Add(start);
		//Debug.LogFormat($"Visit: {start}");
		yield return start;
		edges(start).ForEach(c => {
			candidates.Add(c);
			//Debug.LogFormat($"Added candidate: {c}");

			vertexCandidates[c.to].Add(c);
		});
		antiEdges(start).ForEach(c => vertexCandidates[c].ForEach(cand => candidates.Remove(cand)));
		visit(start);
		steps++;
		if (steps == maxSteps) {
			yield break;
		}

		while (candidates.Count > 0) {
			var current = candidates.Min;
			candidates.Remove(current);
			if (visited.Contains(current.to)) {
				continue;
			}

			visited.Add(current.to);
			//Debug.LogFormat($"Visit: {current.to} (cost {current.weight})");
			if (current.weight > 1) {
				Debug.LogFormat($"High price: {current.weight}");
			}
			yield return current.to;
			edges(current.to).ForEach(c => {
				candidates.Add(c);
				//Debug.LogFormat($"Added candidate: {c}");
				vertexCandidates[c.to].Add(c);
			});
			antiEdges(current.to).ForEach(c => vertexCandidates[c].ForEach(cand => {
				candidates.Remove(cand);
				//Debug.LogFormat($"Removed candidate: {c}");
			}));
			visit(current.to);
			steps++;
			if (steps == maxSteps) {
				yield break;
			}

			if (terminalVertex(current.to)) {
				yield break;
			}
		}
	}
}