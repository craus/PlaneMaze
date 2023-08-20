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

    private static Dictionary<Cell, float> cellPrices = new Dictionary<Cell, float>();

    private static Vector2 Rotate(Vector2 v, float angle) {
        return new Vector2(
            v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle),
            v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle)
        );
    }

    private static float CalculateCellPriceGrid(Vector2Int cell) {
        Vector2 cv = cell;

        return Rand.Range(0, 1f) + 0.5f * Mathf.Min(Mathf.Sin(cv.x / 4f), Mathf.Sin(cv.y / 4f));
    }

    private static float CalculateCellPriceSpiralGrid(Vector2Int cell) {
        Vector2 cv = cell;

        cv = Rotate(cv, cv.magnitude / 10f);

        return Rand.Range(0, 1f) + 0.5f * Mathf.Min(Mathf.Sin(cv.x / 4f), Mathf.Sin(cv.y / 4f));
    }

    //public Color GetCellColor(Vector2Int cell) {
    //    var image = GameManager.instance.mazeSample;
    //    var pixel = image.GetPixel(
    //        (image.width / 2 + cell.x * 4) % image.width,
    //        (image.height / 2 + cell.y * 4) % image.height
    //    );
    //    return pixel.withAlpha(1);
    //}

    //public bool CellInsideImage(Vector2Int cell) {
    //    var image = GameManager.instance.mazeSample;
    //    var x = image.width / 2 + cell.x * 4;
    //    var y = image.height / 2 + cell.y * 4;
    //    return 0 <= x && x < image.width && 0 <= y && y < image.height;
    //}


    //private float CalculateCellPriceImage(Vector2Int cell) {
    //    return Rand.Range(0, 1f) + 5f * GetCellColor(cell).grayscale + (CellInsideImage(cell) ? 0 : 10);
    //}

    private static float CalculateCellPriceRandom(Cell cell) {
        return Rand.Range(0, 1f);
    }

    private static float CalculateCellPriceSinTime(Vector2Int cell) {
        return Rand.Range(0, 1f) + Mathf.Sin(Time.time);
    }

    public static float CellPrice(Cell cell, bool reroll = false) {
        if (!cellPrices.ContainsKey(cell) || reroll) {
            cellPrices[cell] = CalculateCellPriceRandom(cell);
        }
        return cellPrices[cell] + (WorldGenerator.Bad(cell) ? 1 : 0);
    }

}
