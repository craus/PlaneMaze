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
    public GameEvents gameEvents;

    public Unit lastAttackedMonster = null;

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
    public int moveNumber = 0;
    public int ghostSpawnTimeReductionHalfLife = 1000;
    public float ghostSpawnProbabilityPerTurn;

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

    public Metagame Metagame => GameManager.instance.metagame;

    public async void Start() {
        InfoPanel.instance.viewedInfo = new HashSet<IExplainable>();

        mainWorld = Instantiate(boardSample, transform);
        UnityEngine.Debug.LogFormat("New game started");

        player = Instantiate(playerSample, transform);
        player.figure.savePoint = mainWorld.GetCell(Vector2Int.zero);
        await player.figure.Move(mainWorld.GetCell(Vector2Int.zero), isTeleport: true);

        speed = 10000;

        cellOrderList = new List<Cell>();
        await GenerateBiome(Library.instance.dungeon, pauses: false);
        await GenerateBiome(Library.instance.crypt, pauses: false);
        mainWorld.silentMode = true;

        for (int i = 0; i < storeCount; i++) {
            GenerateStore(); 
        }

        monsters.ToList().ForEach(m => {
            m.OnGameStart();
        });

        MusicManager.instance.Switch(MusicManager.instance.playlist);

        await ConfirmationManager.instance.AskConfirmation(
            panel: ConfirmationManager.instance.startPanel, 
            canCancel: false,
            canConfirmByAnyButton: true
        );
    }

    public async Task AskForNextRun() {
        if (await ConfirmationManager.instance.AskConfirmation("Do you want to start another run? If you start, abandon run will be counted as a loss!")) {
            GameManager.instance.RestartGame();
        } else {
            MainUI.instance.QuitApplication();
        }
    }

    public async Task Win() {
        MusicManager.instance.Switch(MusicManager.instance.winPlaylist);
        await ConfirmationManager.instance.AskConfirmation(panel: ConfirmationManager.instance.winPanel, canCancel: false);
        await GameManager.instance.metagame.Win();
        await AskForNextRun();
    }

    public async Task Lose() {
        MusicManager.instance.Switch(MusicManager.instance.losePlaylist);
        if (lastAttackedMonster != null) {
            await ConfirmationManager.instance.AskConfirmation(
                canCancel: false,
                panel: ConfirmationManager.instance.infoPanel,
                customShow: () => InfoPanel.instance.Show(lastAttackedMonster.GetComponent<IExplainable>()),
                canConfirmByAnyButton: true
            );
        }
        await ConfirmationManager.instance.AskConfirmation(panel: ConfirmationManager.instance.losePanel, canCancel: false);
        await GameManager.instance.metagame.Lose();
        await AskForNextRun();
    }

    private void Sell(Cell location, MonoBehaviour sample, int price = -1) {
        var item = GenerateFigure(location, sample);
        if (price == -1) {
            var rules = item.GetComponent<ItemGenerationRules>();
            price = rules != null ? Rand.rnd(rules.minPrice, rules.maxPrice) : 0;
        }
        GenerateFigure(location, paidCellSample).SetPrice((int)Math.Ceiling(price * Metagame.PricesMultiplier));
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
    //    return UnityEngine.Random.Range(0, 1f) + 5f * GetCellColor(cell).grayscale + (CellInsideImage(cell) ? 0 : 10);
    //}

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

    public async void Update() {
        if (Cheats.on) {
            if (Input.GetKeyDown(KeyCode.W)) {
                await Win();
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                await Lose();
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

    public static void Debug(string message) {
        UnityEngine.Debug.LogFormat($"[{instance.time}] [{instance.moveNumber}] {message}");
    }

    public IEnumerable<Cell> AntiEdgesSquare(Cell cell) => cell.Neighbours().Where(MakesSquare).Union(Diagonals(cell).Where(MakesSquare));

    private IEnumerable<Cell> BorderCells(IEnumerable<Cell> cells) {
        var start = cells.MinBy(c => c.position.x).Shift(Vector2Int.left);
        var current = start;
        var direction = Vector2Int.up;
        var result = new HashSet<Cell>();
        for (int i = 0; i < 1000000; i++) {
            if (current.Shift(direction).Wall && current.Shift(direction + direction.RotateRight()).Wall) {
                current = current.Shift(direction + direction.RotateRight());
                direction = direction.RotateRight();
                result.Add(current);
            } else if (current.Shift(direction).Wall) {
                current = current.Shift(direction);
                result.Add(current);
            } else {
                direction = direction.RotateLeft();
            }
        }
        return result;
    }

    private Cell CheapestBorderCell(IEnumerable<Cell> cells) {
        return BorderCells(cells).MinBy((a,b) => CellPrice(a.position) < CellPrice(b.position));
    }

    private async Task GenerateBiome(Biome biome, bool pauses = false) {
        var start = mainWorld.GetCell(Vector2Int.zero);
        if (cellOrderList.Count > 0) {
            start = CheapestBorderCell(cellOrderList);
        }

        var cellOrder = Algorithm.PrimDynamic(
            start: start,
            edges: c => c.Neighbours().Where(c => c.Wall && !MakesCross(c) && !Forbidden(c).Any(f => f.Ordered))
                .Select(c => new Algorithm.Weighted<Cell>(c, CellPrice(c.position))),
            antiEdges: c => Diagonals(c).Where(MakesCross).Union(Forbidden(c)),
            maxSteps: 100000
        ).Take(biome.Size);

        int i = 0;
        foreach (Cell c in cellOrder) {

            cellOrderList.Add(c);
            c.order = cellOrderList.Count-1;
            c.orderInBiome = i;
            c.fieldCell.wall = false;
            c.biome = biome;
            c.UpdateBiome();
            c.UpdateCell();
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

        UnityEngine.Debug.LogFormat($"Cells: {i}");
        UnityEngine.Debug.LogFormat($"Taken Cells Max Price: {cellOrderList.Max(c => CellPrice(c.position))}");
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

    public void AddGem(Cell cell, int amount) {
        if (amount <= 0) {
            return;
        }
        var oldGem = cell.GetFigure<Gem>();
        if (oldGem != null) {
            oldGem.amount += amount;
            oldGem.UpdateSprite();
        } else {
            var newGem = GenerateFigure(cell, gemSample);
            newGem.amount = amount;
            newGem.UpdateSprite();
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
        if (cell.order == 1 && !Metagame.Ascention<NoStartingWeapon>()) {
            GenerateFigure(cell, weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().startingWeight));
            return;
        } else if (cell.order == 0) {
            return;
        } else if (startingItemsSamples.Count() > 0) {
            GenerateFigure(cell, startingItemsSamples.First());
            startingItemsSamples.RemoveAt(0);
            return;
        } else if (cell.biome == Library.instance.crypt && cell.orderInBiome == Library.instance.crypt.Size-1) {
            GenerateFigure(cell, lichSample);
            return;
        }

        if (cell.position.magnitude > 6 && Rand.rndEvent(Metagame.instance.MonsterProbability)) {
            GenerateFigure(cell, cell.biome.monsterSamples.weightedRnd());
        } else if (Rand.rndEvent(0.004f)) {
            GenerateFigure(cell, weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().fieldWeight));
        } else if (Rand.rndEvent(Metagame.HealingPotionSpawnProbability)) {
            GenerateFigure(cell, healingPotionSample);
        } else if (Rand.rndEvent(0)) {
            GenerateFigure(cell, itemSamples.rnd());
        } else if (Rand.rndEvent(0.3f)) {
            GenerateFigure(cell, cell.biome.terrainSamples.weightedRnd());
        }
    }

    private async Task MonstersAndItemsTick(int turnNumber) {
        Debug($"Monsters move");
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

        if (Metagame.SpawnGhosts) {
            await SpawnGhosts();
        }
    }

    private async Task SpawnGhosts() {
        if (Player.instance == null) return;
        if (player.figure.location.board != mainWorld) return;

        ghostSpawnProbabilityPerTurn = 1 - Mathf.Pow(
            1 - Metagame.GhostSpawnProbabilityPerTurn(time),
            Player.instance.figure.location.biome.ghostPower
        );

        for (int i = 0; i < Metagame.MaxGhostSpawnsPerTurn && Rand.rndEvent(ghostSpawnProbabilityPerTurn); i++) {
            await SpawnGhost();
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

    private static bool Highground(
        Cell attackLocation,
        Cell defenceLocation,
        Unit attacker,
        Unit defender
    ) =>
        attackLocation != null &&
        defenceLocation != null &&
        defenceLocation.GetFigure<Hill>() != null && defender.BenefitsFromTerrain &&
        !(attackLocation.GetFigure<Hill>() != null && attacker.BenefitsFromTerrain);

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
            (IsRanged(attackLocation, defenceLocation) || !Highground(attackLocation, defenceLocation, attacker, defender));
    }
}
