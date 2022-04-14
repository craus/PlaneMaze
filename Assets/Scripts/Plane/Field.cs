﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Field : MonoBehaviour
{
    SparseCollections.Sparse2DMatrix<int, int, FieldCell> map = new SparseCollections.Sparse2DMatrix<int, int, FieldCell>();
    RandomField rightCost = new RandomField();
    RandomField upCost = new RandomField();
    RandomField core = new RandomField();
    RandomField wall = new RandomField();

    SparseCollections.Sparse2DMatrix<int, int, Color> biome = new SparseCollections.Sparse2DMatrix<int, int, Color>();

    public FieldCell this[int x, int y]
    {
        get
        {
            if (map[x, y] == null) {
                map[x,y] = Generate(x, y);
            }
            return map[x, y];
        }
    }

    private IEnumerable<Algorithm.Weighted<(int, int)>> Edges((int, int) from) {
        yield return new Algorithm.Weighted<(int, int)>((from.Item1 + 1, from.Item2), rightCost[from.Item1, from.Item2]);
        yield return new Algorithm.Weighted<(int, int)>((from.Item1 - 1, from.Item2), rightCost[from.Item1 - 1, from.Item2]);
        yield return new Algorithm.Weighted<(int, int)>((from.Item1, from.Item2 + 1), upCost[from.Item1, from.Item2]);
        yield return new Algorithm.Weighted<(int, int)>((from.Item1, from.Item2 - 1), upCost[from.Item1, from.Item2 - 1]);
    }

    private bool Core(int x, int y) {
        return core[x, y] < 0.00001f;
    }

    private Color GenerateNewBiome() {
        return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }

    private void EnsureBiome(int x, int y) {
        if (!biome.ContainsKey(x, y)) {

            List<(int, int)> visitedCells = new List<(int, int)>();

            Algorithm.Prim(
                start: (x, y),
                edges: Edges,
                terminalVertex: v => biome.ContainsKey(v.x, v.y) || Core(v.x, v.y),
                visit: v => visitedCells.Add(v)
            );

            var end = visitedCells.Last();
            if (!biome.ContainsKey(end.Item1, end.Item2)) {
                biome[end.Item1, end.Item2] = GenerateNewBiome();
            }
            visitedCells.ForEach(v => biome[v.Item1, v.Item2] = biome[end.Item1, end.Item2]);
        }
    }

    private bool Wall(int x, int y) {
        float horizontal = 1;

        int delta = 3;

        for (int i = -delta; i <= delta; i++) {
            horizontal = Mathf.Min(horizontal, wall[x + i, y]);
        }

        float vertical = 1;
        for (int i = -delta; i <= delta; i++) {
            vertical = Mathf.Min(vertical, wall[x, y + i]);
        }

        float threshold = 0.2f;

        return horizontal >= threshold || vertical >= threshold;
    }

    private bool StrongerThanNeighbours(int x, int y) {
        float neighbours = 0;
        neighbours += wall[x + 1, y];
        neighbours += wall[x - 1, y];
        neighbours += wall[x, y + 1];
        neighbours += wall[x, y - 1];
        return wall[x, y] > neighbours;
    }

    private bool RandomBalanced(int x, int y) {
        return Rand.rndEvent(1 / (1 + Mathf.Sqrt(2)));
    }

    private FieldCell Generate(int x, int y) {
        var result = new FieldCell();
        //result.wall =  ? true : false;

        //EnsureBiome(x, y);

        result.wall = RandomBalanced(x, y);
            //? Color.black : Color.white;

        return result;
    }
}
