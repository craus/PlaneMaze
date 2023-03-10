using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    public static Game instance => GameManager.instance ? GameManager.instance.game : null;

    public List<Ascention> ascentions;

    public Player playerSample;
    public Player player;

    public int storeCount = 4;
    public int storeRadius = 5;

    public List<Monster> monsterSamples;

    public List<Monster> monsters;

    public List<Weapon> weaponSamples;
    public List<Item> itemSamples;
    public List<Figure> startingItemsSamples;

    public List<Store> storeSamples;
    public List<Weighted<Figure>> terrainSamples;
    public Portal portalSample;
    public HealingPotion healingPotionSample;
    public PaidCell paidCellSample;
    public Info infoSample;
    public RingOfTerraforming ringOfTerraformingSample;

    public Ghost ghostSample;
    public Lich lichSample;
    public Gem gemSample;

    public List<Cell> cellOrderList;
    public int unlockedCells = (int)1e9;

    public int time = 0;
    public int ghostSpawnTimeReductionHalfLife = 1000;
    public float ghostSpawnProbabilityPerTurn;

    public int worldSize = 1000;

    public Board boardSample;
    public Board mainWorld;
    public List<Board> stores;

    public List<Func<int, Task>> afterPlayerMove = new List<Func<int, Task>>();
    public List<Func<int, Task>> afterMonsterMove = new List<Func<int, Task>>();

    public HashSet<(Cell, Cell)> contaminations = new HashSet<(Cell, Cell)>();
    public HashSet<Cell> clearedCells = new HashSet<Cell>();

    public Map<int, TaskCompletionSource<bool>> completedTurns = new Map<int, TaskCompletionSource<bool>>(() => new TaskCompletionSource<bool>());

    internal bool Ascention<T>() where T: Ascention => ascentions.Any(a => a is T);
    internal int Ascentions<T>() where T: Ascention => ascentions.Count(a => a is T);

    public GameObject startPanel;
    public GameObject winPanel;
    public GameObject losePanel;

    public bool win = false;
    public bool lose = false;

    public async void Start() {
        mainWorld = Instantiate(boardSample, transform);
        Debug.LogFormat("New game started");

        player = Instantiate(playerSample, transform);
        player.figure.savePoint = mainWorld.GetCell(Vector2Int.zero);
        await player.figure.Move(mainWorld.GetCell(Vector2Int.zero), isTeleport: true);

        speed = 10000;
        await EnumerateCells(worldSize, pauses: true);


        for (int i = 0; i < storeCount; i++) {
            GenerateStore(); 
        }

        monsters.ToList().ForEach(m => {
            m.OnGameStart();
        });

        MusicManager.instance.Switch(MusicManager.instance.playlist);

        startPanel.SetActive(true);
    }

    public void ClosePanel() {
        startPanel.SetActive(false);
        losePanel.SetActive(false);
        winPanel.SetActive(false);
        InfoPanel.instance.panel.SetActive(false);

        if (win || lose) {
            GameManager.instance.Restart();
        }
    }

    public async Task Win() {
        win = true;
        MusicManager.instance.Switch(MusicManager.instance.winPlaylist);
        winPanel.SetActive(true);
    }

    public async Task Lose() {
        lose = true;
        MusicManager.instance.Switch(MusicManager.instance.losePlaylist);
        losePanel.SetActive(true);
    }

    private void Sell(Cell location, MonoBehaviour sample, int price = -1) {
        var item = GenerateFigure(location, sample);
        if (price == -1) {
            var rules = item.GetComponent<ItemGenerationRules>();
            price = rules != null ? Rand.rnd(rules.minPrice, rules.maxPrice) : 0;
        }
        GenerateFigure(location, paidCellSample).SetPrice(price);
        var info = GenerateFigure(location.Shift(Vector2Int.down), infoSample);
        info.explainable = item.GetComponent<IExplainable>();
    }

    private void GenerateStore() {
        var newStore = Instantiate(boardSample, transform);
        newStore.silentMode = true;
        newStore.GetCell(Vector2Int.zero).Vicinity(storeRadius + 1).ForEach(cell => cell.gameObject.SetActive(true));
        newStore.GetCell(Vector2Int.zero).Vicinity(storeRadius).ForEach(cell => {
            cell.fieldCell.wall = false;
            cell.UpdateCell();
        });
        newStore.gameObject.SetActive(false);
        stores.Add(newStore);

        var entry = GenerateFigure(mainWorld.cells.Where(c => c.figures.Count() == 0 && !c.Wall).Rnd(), portalSample);
        var exit = GenerateFigure(newStore.GetCell(Vector2Int.zero), portalSample);
        entry.second = exit;
        exit.second = entry;

        Sell(newStore[-2, 3], healingPotionSample);
        Sell(newStore[0, 3], weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().storeWeight));
        Sell(newStore[2, 3], ringOfTerraformingSample);

        Sell(newStore[-2, -2], itemSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().storeWeight));
        Sell(newStore[0, -2], itemSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().storeWeight));
        Sell(newStore[2, -2], itemSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().storeWeight));
    }

    private void SecondStep(Cell cell) {
        return;
        if (!cell.Free || cell.Neighbours8().Any(c => !c.Free)) {
            return;
        }
        if (Rand.rndEvent(0.05f)) {
            GenerateFigure(cell, storeSamples.rnd());
        }
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

        if (Input.GetKeyDown(KeyCode.Return)) {
            if (startPanel.activeSelf || winPanel.activeSelf || losePanel.activeSelf) {
                ClosePanel();
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
            start: mainWorld.GetCell(Vector2Int.zero),
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

        mainWorld.silentMode = true;

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

    public T GenerateFigure<T>(Cell cell, T sample) where T: MonoBehaviour {
        var f = Instantiate(sample);
        _ = f.GetComponent<Figure>().Move(cell, isTeleport: true);

        var explainable = f.GetComponent<IExplainable>();
        if (explainable != null) {
            explainable.Sample = sample.GetComponent<IExplainable>();
        }

        var monster = f.GetComponent<Monster>();
        if (monster != null) {
            monsters.Add(monster);
        }

        return f;
    }

    public void AfterCellAdded(Cell cell) {
        if (cell.order == 1) {
            GenerateFigure(cell, weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().startingWeight));
            return;
        } else if (cell.order == 0) {
            return;
        } else if (startingItemsSamples.Count() > 0) {
            GenerateFigure(cell, startingItemsSamples.First());
            startingItemsSamples.RemoveAt(0);
            return;
        } else if (cell.order == worldSize - 1) {
            GenerateFigure(cell, lichSample);
            return;
        }

        if (cell.position.magnitude > 6 && Rand.rndEvent(0.1f)) {
            GenerateFigure(cell, monsterSamples.rnd());
        } else if (Rand.rndEvent(0.004f)) {
            GenerateFigure(cell, weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().fieldWeight));
        } else if (Rand.rndEvent(0)) {
            GenerateFigure(cell, itemSamples.rnd());
        } else if (Rand.rndEvent(0.3f)) {
            GenerateFigure(cell, terrainSamples.weightedRnd());
        }
    }

    private async Task MonstersAndItemsTick(int turnNumber) {
        Debug.LogFormat($"[{time}] Monsters move");
        await Task.WhenAll(monsters.ToList().Select(m => m.Move()).Concat(afterPlayerMove.Select(listener => listener(turnNumber))));
        if (this == null) {
            return;
        }
        await Task.WhenAll(afterMonsterMove.Select(listener => listener(turnNumber)));
    }

    public async Task AfterPlayerMove() {
        await MonstersAndItemsTick(time);
        completedTurns[time].SetResult(true);
        time++;

        if (Ascention<GhostSpawns>()) {
            if (player.figure.location.board == mainWorld) {
                ghostSpawnProbabilityPerTurn = 1 - Mathf.Pow(0.5f, time * 1f / ghostSpawnTimeReductionHalfLife);
                ghostSpawnProbabilityPerTurn /= 8;
                ghostSpawnProbabilityPerTurn *= Mathf.Pow(2, Ascentions<DoubleGhostSpawns>());
                for (int i = 0; i < 4 && Rand.rndEvent(ghostSpawnProbabilityPerTurn); i++) {
                    await SpawnGhost();
                }
            }
        }
    }

    private async Task SpawnGhost() {
        var center = Player.instance.figure.location;
        GenerateFigure(Rand.rndExcept(center.Vicinity(11).Where(c => c.Free).ToList(), center.Vicinity(7)), ghostSample);
    }

    private static bool CanAttack(Unit attacker) =>
        attacker == null ||
        attacker.figure.location.GetFigure<PeaceTrap>() == null &&
        !attacker.GetComponent<Disarm>().Active;

    private static bool CanBeAttacked(Unit defender, Weapon weapon) => 
        defender != null && 
        defender.Vulnerable &&
        !defender.figure.location.Wall;

    private static bool IsRanged(Cell attackLocation, Cell defenceLocation) =>
        attackLocation != null &&
        defenceLocation != null &&
        (defenceLocation.position - attackLocation.position).MaxDelta() >= 2;

    private static bool Highground(Cell attackLocation, Cell defenceLocation) =>
        attackLocation != null &&
        defenceLocation != null &&
        defenceLocation.GetFigure<Hill>() != null &&
        attackLocation.GetFigure<Hill>() == null;

    public static bool CanAttack(
        Unit attacker, 
        Unit defender, 
        Weapon weapon = null, 
        Cell attackLocation = null,
        Cell defenceLocation = null
    ) {
        if (attacker) attackLocation ??= attacker.figure.location;
        if (defender) defenceLocation ??= defender.figure.location;

        return
            CanAttack(attacker) &&
            CanBeAttacked(defender, weapon) &&
            (IsRanged(attackLocation, defenceLocation) || !Highground(attackLocation, defenceLocation));
    }
}
