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

    public static bool fasterMonsters;
    public static bool monstersRegenerate;

    public Player playerSample;
    public Player player;
    public GameEvents gameEvents;

    public int storeCount = 4;
    public int storeRadius = 5;

    public List<Weapon> weaponSamples;
    public List<Item> itemSamples;
    public List<Figure> startingItemsSamples;

    public List<Store> storeSamples;
    public Portal portalSample;
    public HealingPotion healingPotionSample;
    public PaidCell paidCellSample;
    public Info infoSample;
    public RingOfTerraforming ringOfTerraformingSample;

    public Ghost ghostSample;
    public Lich lichSample;
    public Gem gemSample;

    public List<Cell> cellOrderList;
    public Biome bossBiome;
    public List<Biome> biomesOrder;

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

    public Metagame Metagame => GameManager.instance.metagame;

    public DateTime startTime;

    public bool gameOver = false;

    public void Awake() {
        new ValueTracker<int>(() => time, v => {
            time = v;
            completedTurns.Clear();
        });
        new ValueTracker<int>(() => moveNumber, v => moveNumber = v);
        new ValueTracker<bool>(() => gameOver, v => gameOver = v);
        MusicManager.instance.CreateValueTrackers();

        fasterMonsters = Metagame.instance.HasAscention<FasterMonsters>();
        monstersRegenerate = Metagame.instance.HasAscention<MonstersHeal>();
    }

    public async void Start() {
        startTime = DateTime.Now;

        InfoPanel.instance.viewedInfo = new HashSet<IExplainable>();

        mainWorld = Instantiate(boardSample, transform);
        mainWorld.currentBiome = Library.instance.dungeon;
        UnityEngine.Debug.LogFormat("New game started");


        speed = 10000;

        //await GenerateTestWorld();
        await GenerateWorld();

        mainWorld.silentMode = true;

        mainWorld.movables.ToList().ForEach(m => {
            m.OnGameStart();
        });

        MusicManager.instance.Switch(MusicManager.instance.playlist);
        UndoManager.instance.Save();

        await ConfirmationManager.instance.AskConfirmation(
            panel: ConfirmationManager.instance.startPanel, 
            canCancel: false,
            canConfirmByAnyButton: true
        );
    }

    private async Task GenerateTestWorld() {
        AddFloorCell(mainWorld.GetCell(Vector2Int.zero));
        AddFloorCell(mainWorld.GetCell(new Vector2Int(0, 1)));
        AddFloorCell(mainWorld.GetCell(new Vector2Int(0, 2)));
        player = GenerateFigure(playerSample, 0, 0);
        GenerateFigure(Library.Get<BackArmor>(), 0, 1);
        GenerateFigure(Library.Get<Harpy>(), 0, 2);
    }

    private async Task GenerateWorld() {
        cellOrderList = new List<Cell>();
        bossBiome = Library.instance.bossBiomes.Rnd();

        biomesOrder = Library.instance.biomes.Shuffled();

        foreach (var biome in biomesOrder) {
            await GenerateBiome(biome);
        }

        var start = cellOrderList.First(cell => cell.biome == Library.instance.dungeon && cell.orderInBiome == 0);
        player = GenerateFigure(start, playerSample);

        foreach (var cell in cellOrderList) PopulateCell(cell);

        if (bossBiome == Library.instance.darkrootForest) {
            var witch = GenerateFigure(
                cellOrderList.Where(cell => cell.biome == Library.instance.darkrootForest && cell.figures.Count() == 0).Rnd(),
                Library.instance.darkrootForest.GetComponent<DarkrootForest>().witch);
            var sister = GenerateFigure(
                cellOrderList.Where(cell => cell.biome == Library.instance.darkrootForest && cell.figures.Count() == 0).Rnd(),
                Library.instance.darkrootForest.GetComponent<DarkrootForest>().sister);

            witch.witch = witch;
            witch.sister = sister;
            sister.witch = witch;
            sister.sister = sister;
        }

        var forestBorder = BorderCells(cellOrderList.Where(cell => cell.biome == Library.instance.darkrootForest)).Where(cell => cell.Wall);
        foreach (var cell in forestBorder) {
            AddFloorCell(cell);
            GenerateFigure(cell, Library.instance.tree);
        }

        for (int i = 0; i < storeCount; i++) {
            GenerateStore();
        }

        foreach (var cell in cellOrderList) {
            cell.AfterStoresAdded();
        }
    }

    public async Task AskForNextRun() {
        if (await ConfirmationManager.instance.AskConfirmation("Do you want to start another run? If you start, abandon run will be counted as a loss!")) {
            GameManager.instance.RestartGame();
        } else {
            MainUI.instance.QuitApplication();
        }
    }

    public async Task Win() {
        if (gameOver) return;
        gameOver = true;
        MusicManager.instance.Switch(MusicManager.instance.winPlaylist);
        await ConfirmationManager.instance.AskConfirmation(panel: ConfirmationManager.instance.winPanel, canCancel: false);
        await GameManager.instance.metagame.Win();
        await AskForNextRun();
    }

    public async Task Lose() {
        if (gameOver) return;
        gameOver = true;
        MusicManager.instance.Switch(MusicManager.instance.losePlaylist);
        if (Player.instance.lastAttacker != null) {
            await ConfirmationManager.instance.AskConfirmation(
                canCancel: false,
                panel: ConfirmationManager.instance.infoPanel,
                customShow: () => InfoPanel.instance.Show(
                    Player.instance.lastAttacker.GetComponent<IExplainable>(),
                    repeatable: true
                ),
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
        newStore.currentBiome = Library.instance.dungeon;
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


    public static void Debug(string message) {
        UnityEngine.Debug.LogFormat($"[{instance.time}] [{instance.moveNumber}] {message}");
    }

    public List<Cell> border;
    private IEnumerable<Cell> BorderCells(IEnumerable<Cell> cells) {
        HashSet<Cell> inside = new HashSet<Cell>(cells);
        Func<Cell, bool> outer = c => !inside.Contains(c);

        var start = cells.MinBy(c => c.position.x).Shift(Vector2Int.left);
        var current = start;
        var direction = Vector2Int.up;
        var result = new HashSet<Cell>();
        for (int i = 0; i < 1000000; i++) {
            if (outer(current.Shift(direction)) && outer(current.Shift(direction + direction.RotateRight()))) {
                current = current.Shift(direction + direction.RotateRight());
                direction = direction.RotateRight();
                result.Add(current);
                border.Add(current);
            } else if (outer(current.Shift(direction))) {
                current = current.Shift(direction);
                result.Add(current);
                border.Add(current);
            } else {
                direction = direction.RotateLeft();
            }
            if (current == start) {
                return result;
            }
        }
        UnityEngine.Debug.LogError("Too many iterations for border!");
        return result;
    }

    private Cell CheapestBorderCell(IEnumerable<Cell> cells) {
        var border = BorderCells(cells);
        UnityEngine.Debug.LogFormat($"border: {border.ExtToString()}");
        Map<Cell, float> fixedPrices = new Map<Cell, float>();
        foreach (var c in border) {
            fixedPrices[c] = WorldGenerator.CellPrice(c);
        }
        return border.MinBy((a,b) => fixedPrices[a] < fixedPrices[b]);
    }

    private async Task GenerateBiome(Biome biome, bool pauses = false) {
        var boss = bossBiome == biome;
        mainWorld.currentBiome = biome;

        var start = mainWorld.GetCell(Vector2Int.zero);
        if (cellOrderList.Count > 0) {
            start = CheapestBorderCell(cellOrderList);
            Debug($"Start of biome: {start}");
        }

        var cellOrder = Algorithm.PrimDynamic(
            start: start,
            edges: c => c.Neighbours().Where(c => c.Wall)
                .Select(c => new Weighted<Cell>(c, WorldGenerator.CellPrice(c, reroll: true))),
            maxSteps: 100000
        ).Take(biome.Size);

        var biomeCells = new List<Cell>();

        int i = 0;
        foreach (Cell c in cellOrder) {

            c.orderInBiome = i;
            c.biome = biome;
            biomeCells.Add(c);
            c.UpdateBiome();

            AddFloorCell(c);

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
        UnityEngine.Debug.LogFormat($"Taken Cells Max Price: {cellOrderList.Max(c => WorldGenerator.CellPrice(c))}");
    }

    private void AddFloorCell(Cell c) {
        cellOrderList.Add(c);
        c.order = cellOrderList.Count - 1;
        c.fieldCell.wall = false;
        c.UpdateCell();
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

    public T GenerateFigure<T>(T sample, int x, int y, Board board = null) where T : MonoBehaviour {
        board ??= mainWorld;
        return GenerateFigure(board.GetCell(new Vector2Int(x, y)), sample);
    }

    public Map<Figure, List<Figure>> generatedFigures = new Map<Figure, List<Figure>>(() => new List<Figure>());
    public T GenerateFigure<T>(Cell cell, T sample) where T: MonoBehaviour {
        var f = Instantiate(sample);
        f.gameObject.name = $"{sample.gameObject.name} #{generatedFigures[sample.GetComponent<Figure>()].Count}";
        generatedFigures[sample.GetComponent<Figure>()].Add(f.GetComponent<Figure>());

        if (f.GetComponent<SampleTracker>() != null) {
            f.GetComponent<SampleTracker>().createdFromSample = sample.GetComponent<SampleTracker>();
        }

        _ = f.GetComponent<Figure>().Move(cell, isTeleport: true);

        var explainable = f.GetComponent<IExplainable>();
        if (explainable != null) {
            explainable.Sample = sample.GetComponent<IExplainable>();
        }

        var iMovable = f.GetComponent<IMovable>();
        if (iMovable != null) {
            cell.board.movables.Add(iMovable);
        }

        return f;
    }

    public void PopulateCell(Cell cell) {
        if (cell.orderInBiome == 1 && cell.biome == Library.instance.dungeon && !Metagame.HasAscention<NoStartingWeapon>()) {
            GenerateFigure(cell, weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().startingWeight));
            return;
        } else if (cell.biome == Library.instance.dungeon && startingItemsSamples.Count() > 0) {
            GenerateFigure(cell, startingItemsSamples.First());
            startingItemsSamples.RemoveAt(0);
            return;
        } else if (cell.biome == Library.instance.crypt && cell.orderInBiome == Library.instance.crypt.Size-1 && bossBiome == Library.instance.crypt) {
            GenerateFigure(cell, lichSample);
            return;
        }

        if (cell.biome == Library.instance.darkrootForest && Rand.rndEvent(0.1f)) {
            GenerateFigure(cell, Library.instance.tree);
        } else if ((player.figure.Location.position - cell.position).magnitude > 6 && Rand.rndEvent(Metagame.instance.MonsterProbability)) {
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
        await Task.WhenAll(Player.instance.figure.Location.board.movables.ToList().Select(m => m.Move()).Concat(afterPlayerMove.Select(listener => listener(turnNumber))));
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
        if (player.figure.Location.board != mainWorld) return;

        ghostSpawnProbabilityPerTurn = 1 - Mathf.Pow(
            1 - Metagame.GhostSpawnProbabilityPerTurn(time),
            Player.instance.figure.Location.biome.ghostPower
        );

        for (int i = 0; i < Metagame.MaxGhostSpawnsPerTurn && Rand.rndEvent(ghostSpawnProbabilityPerTurn); i++) {
            await SpawnGhost();
        }
    }

    private async Task SpawnGhost() {
        var center = Player.instance.figure.Location;
        GenerateFigure(Rand.rndExcept(center.Vicinity(11).Where(c => c.Free).ToList(), center.Vicinity(7)), ghostSample);
    }

    private static bool CanAttack(Unit attacker) =>
        attacker == null ||
        attacker.figure.Location.GetFigure<PeaceTrap>() == null &&
        !attacker.GetComponent<Disarm>().Active;

    private static bool CanBeAttacked(Unit defender, Weapon weapon) => 
        defender != null && 
        defender.Vulnerable &&
        !defender.figure.Location.Wall;

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
        if (attacker) attackLocation ??= attacker.figure.Location;
        if (defender) defenceLocation ??= defender.figure.Location;

        return
            CanAttack(attacker) &&
            CanBeAttacked(defender, weapon) &&
            (IsRanged(attackLocation, defenceLocation) || !Highground(attackLocation, defenceLocation, attacker, defender));
    }
}
