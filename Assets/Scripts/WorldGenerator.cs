using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public static class WorldGenerator
{
    private static bool MakesCross(Cell cell, Vector2Int direction) =>
        cell.Shift(direction + direction.RotateRight()).Ordered &&
        !cell.Shift(direction).Ordered &&
        !cell.Shift(direction.RotateRight()).Ordered;

    private static bool MakesCross(Cell cell) =>
        MakesCross(cell, Vector2Int.up) ||
        MakesCross(cell, Vector2Int.right) ||
        MakesCross(cell, Vector2Int.down) ||
        MakesCross(cell, Vector2Int.left);

    private static bool MakesSquare(Cell cell, Vector2Int direction) => cell.Shift(direction + direction.RotateRight()).Ordered && cell.Shift(direction).Ordered && cell.Shift(direction.RotateRight()).Ordered;
    private static bool MakesSquare(Cell cell) => MakesSquare(cell, Vector2Int.up) || MakesSquare(cell, Vector2Int.right) || MakesSquare(cell, Vector2Int.down) || MakesSquare(cell, Vector2Int.left);


    public static IEnumerable<Cell> Diagonals(Cell cell) {
        yield return cell.Shift(1, 1);
        yield return cell.Shift(1, -1);
        yield return cell.Shift(-1, -1);
        yield return cell.Shift(-1, 1);
    }

    public static IEnumerable<Cell> Forbidden(Cell cell) {
        yield break;
        yield return cell.Shift(5, 0);
        yield return cell.Shift(-5, 0);
        yield return cell.Shift(0, 5);
        yield return cell.Shift(0, -5);
    }
    public static IEnumerable<Cell> AntiEdgesSquare(Cell cell) => cell.Neighbours().Where(MakesSquare).Union(Diagonals(cell).Where(MakesSquare));

    public static bool Bad(Cell c) => !MakesCross(c) && !Forbidden(c).Any(f => f.Ordered);

}
