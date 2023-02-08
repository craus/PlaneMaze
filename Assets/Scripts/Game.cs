using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance => GameManager.instance.game;

    public Player playerSample;
    public Player player;


    public Monster monsterSample;
    public List<Monster> monsters;

    public Dagger daggerSample;
    public Stiletto stilettoSample;

    public List<Cell> cellOrderList;
    public int unlockedCells = (int)1e9;

    public int time = 0;

    public Board boardSample;
    public Board board;

    public Gem gemSample;
    public Gem gem;

    public Transform figureParent;

    public HashSet<(Cell, Cell)> contaminations = new HashSet<(Cell, Cell)>();
    public HashSet<Cell> clearedCells = new HashSet<Cell>();
    public HashSet<Gem> gems = new HashSet<Gem>();

    public async void Start() {
        board = Instantiate(boardSample, transform);
        Debug.LogFormat("New game started");
        
        speed = 10000;
        await EnumerateCells(1000, pauses: true);

        player = Instantiate(playerSample, transform);
        player.figure.savePoint = board.GetCell(Vector2Int.zero);
        player.figure.Move(board.GetCell(Vector2Int.zero), isTeleport: true);

        GenerateFigure(cellOrderList[12], daggerSample);
        GenerateFigure(cellOrderList[33], stilettoSample);
        

        //PlaceGem();

        //player.figure.location.Dark = false;
        //player.figure.location.UpdateCell();
    }

    private Dictionary<Vector2Int, float> cellPrices = new Dictionary<Vector2Int, float>();

    private Vector2 Rotate(Vector2 v, float angle) {
        return new Vector2(
            v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle),
            v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle)
        );
    }

    private float CalculateCellPriceGrid(Vector2Int cell) {
        Vector2 cv = cell;

        return UnityEngine.Random.Range(0, 1f) + 0.5f * Mathf.Min(Mathf.Sin(cv.x / 4f), Mathf.Sin(cv.y / 4f));
    }

    private float CalculateCellPriceSpiralGrid(Vector2Int cell) {
        Vector2 cv = cell;

        cv = Rotate(cv, cv.magnitude / 10f);

        return UnityEngine.Random.Range(0, 1f) + 0.5f * Mathf.Min(Mathf.Sin(cv.x / 4f), Mathf.Sin(cv.y / 4f));
    }

    public Color GetCellColor(Vector2Int cell) {
        var image = GameManager.instance.mazeSample;
        var pixel = image.GetPixel(
            (image.width / 2 + cell.x * 4) % image.width,
            (image.height / 2 + cell.y * 4) % image.height
        );
        return pixel.withAlpha(1);
    }

    public bool CellInsideImage(Vector2Int cell) {
        var image = GameManager.instance.mazeSample;
        var x = image.width / 2 + cell.x * 4;
        var y = image.height / 2 + cell.y * 4;
        return 0 <= x && x < image.width && 0 <= y && y < image.height;
    }


    private float CalculateCellPriceImage(Vector2Int cell) {
        return UnityEngine.Random.Range(0, 1f) + 5f * GetCellColor(cell).grayscale + (CellInsideImage(cell) ? 0 : 10);
    }

    private float CalculateCellPriceRandom(Vector2Int cell) {
        return UnityEngine.Random.Range(0, 1f);
    }

    private float CalculateCellPriceSinTime(Vector2Int cell) {
        return UnityEngine.Random.Range(0, 1f) + Mathf.Sin(Time.time);
    }

    public float CellPrice(Vector2Int cell) {
        if (!cellPrices.ContainsKey(cell) || true) {
            cellPrices[cell] = CalculateCellPriceRandom(cell);
        }
        return cellPrices[cell];
    }

    public float speed = 1;
    public Task commandToContinue;

    private async Task CommandToContinue() {
        if (speed * Time.deltaTime > 1) {
            await Task.Delay(1);
        } else {
            await Task.Delay((int)(1000 / speed));
        }
        //commandToContinue = new Task(() => { });
        //await commandToContinue;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (commandToContinue != null) {
                commandToContinue.RunSynchronously();
            }
        }
    }

    private bool MakesCross(Cell cell, Vector2Int direction) => cell.Shift(direction + direction.RotateRight()).Ordered && !cell.Shift(direction).Ordered && !cell.Shift(direction.RotateRight()).Ordered;
    private bool MakesCross(Cell cell) => MakesCross(cell, Vector2Int.up) || MakesCross(cell, Vector2Int.right) || MakesCross(cell, Vector2Int.down) || MakesCross(cell, Vector2Int.left);

    private bool MakesSquare(Cell cell, Vector2Int direction) => cell.Shift(direction + direction.RotateRight()).Ordered && cell.Shift(direction).Ordered && cell.Shift(direction.RotateRight()).Ordered;
    private bool MakesSquare(Cell cell) => MakesSquare(cell, Vector2Int.up) || MakesSquare(cell, Vector2Int.right) || MakesSquare(cell, Vector2Int.down) || MakesSquare(cell, Vector2Int.left);


    public IEnumerable<Cell> Diagonals(Cell cell) {
        yield return cell.Shift(1, 1);
        yield return cell.Shift(1, -1);
        yield return cell.Shift(-1, -1);
        yield return cell.Shift(-1, 1);
    }

    public IEnumerable<Cell> Forbidden(Cell cell) {
        yield break;
        yield return cell.Shift(5, 0);
        yield return cell.Shift(-5, 0);
        yield return cell.Shift(0, 5);
        yield return cell.Shift(0, -5);
    }

    public IEnumerable<Cell> AntiEdgesSquare(Cell cell) => cell.Neighbours().Where(MakesSquare).Union(Diagonals(cell).Where(MakesSquare));

    private async Task EnumerateCells(int cnt, bool pauses = false) {

        var cellOrder = Algorithm.PrimDynamic(
            start: board.GetCell(Vector2Int.zero),
            edges: c => c.Neighbours().Where(c => !MakesCross(c) && !Forbidden(c).Any(f => f.Ordered)).Select(c => new Algorithm.Weighted<Cell>(c, CellPrice(c.position))),
            antiEdges: c => Diagonals(c).Where(MakesCross).Union(Forbidden(c)),
            maxSteps: 100000
        ).Take(cnt);

        cellOrderList = new List<Cell>();
        int i = 0;
        foreach (Cell c in cellOrder) {

            c.order = i;
            c.fieldCell.wall = false;
            c.UpdateCell();
            cellOrderList.Add(c);
            AfterCellAdded(c);
            CameraControl.instance.followPoint = true;
            CameraControl.instance.pointToFollow = c.transform.position;

            if (pauses) {
                int iterations = Math.Max(1, (int)(speed * Time.deltaTime));
                if (i % iterations == 0) {
                    await CommandToContinue();
                }
            }

            if (Game.instance == null) {
                return;
            }

            ++i;
        }
        CameraControl.instance.followPoint = false;

        Debug.LogFormat($"Cells: {i}");
        Debug.LogFormat($"Taken Cells Max Price: {cellOrderList.Max(c => CellPrice(c.position))}");
    }

    public void Contaminate(Cell cell) {
        if (cell.figures.Any(f => f.GetComponent<Player>())) {
            return;
        }
        if (cell.figures.Any(f => f.GetComponent<Wall>())) {
            if (Rand.rndEvent(0.5f)) {
                return;
            }
            cell.figures.First(f => f.GetComponent<Wall>()).GetComponent<Wall>().Hit();
            return;
        }
        cell.Dark = true;
        foreach (var f in cell.figures.Where(f => f.GetComponent<Building>())) {
            Destroy(f.gameObject);
        }
    }

    private T GenerateFigure<T>(Cell cell, T sample) where T: MonoBehaviour {
        var f = Instantiate(sample);
        f.GetComponent<Figure>().Move(cell);
        f.transform.SetParent(figureParent);
        return f;
    }

    public void AfterCellAdded(Cell cell) {
        if (Rand.rndEvent(0.1f)) {
            monsters.Add(GenerateFigure(cell, monsterSample));
        }
    }

    public void AfterPlayerMove() {
        monsters.ForEach(m => m.Move());
        //player.figure.location.Dark = false;

        //foreach (var c in contaminations.ToList()) {
        //    if (Rand.rndEvent(0.02f) && !c.Item1.Locked) {
        //        Contaminate(c.Item2);
        //    }
        //}

        foreach (var c in clearedCells.ToList()) {
            if (c.figures.Count() == 0) {
                if (Rand.rndEvent(0.00008f)) {
                    //var gem = Instantiate(gemSample, figureParent);
                    //gem.GetComponent<Figure>().Move(c);
                    //gems.Add(gem);
                    ++player.gems;
                }
            }
        }
        if (Rand.rndEvent(0.008f)) {
            ++player.gems;
        }

        //foreach (var g in gems.ToList()) {
        //    if (Rand.rndEvent(0.02f)) {
        //        Destroy(g.gameObject);
        //        gems.Remove(g);
        //    }
        //}

        time++;
    }

    private void UpdateContamination(Cell from, Cell to) {
        //if (from.Dark && !to.Dark) {
        //    contaminations.Add((from, to));
        //} else {
        //    contaminations.Remove((from, to));
        //}
    }

    public void OnCellDarknessChanged(Cell cell) {
        //foreach (var n in cell.Neighbours().Where(c => !c.Wall)) {
        //    UpdateContamination(cell, n);
        //    UpdateContamination(n, cell);
        //}
        //if (cell.Dark) {
        //    clearedCells.Remove(cell);
        //} else {
        //    clearedCells.Add(cell);
        //}
        //if (unlockedCells < clearedCells.Count() * 2) {
        //    unlockedCells = clearedCells.Count() * 2;
        //    cellOrderList.Take(unlockedCells).ForEach(c => c.UpdateCell());
        //}
    }
}
