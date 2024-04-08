using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class WorldGenerator : Singletone<WorldGenerator>
{
    public int storeCount = 4;
    public int storeRadius = 5;

    public List<Cell> cellOrderList;
    public Biome bossBiome;
    public Biome startBiome;
    public List<Biome> biomesOrder;

    private static bool MakesCross(Cell cell, Vector2Int direction) =>
        cell.Wall == cell.Shift(direction + direction.RotateRight()).Wall &&
        cell.Wall != cell.Shift(direction).Wall &&
        cell.Wall != cell.Shift(direction.RotateRight()).Wall;

    private static bool MakesCross(Cell cell) =>
        MakesCross(cell, Vector2Int.up) ||
        MakesCross(cell, Vector2Int.right) ||
        MakesCross(cell, Vector2Int.down) ||
        MakesCross(cell, Vector2Int.left);

    private static bool MakesSquare(Cell cell, Vector2Int direction) => 
        cell.Shift(direction + direction.RotateRight()).Wall && 
        cell.Shift(direction).Wall && 
        cell.Shift(direction.RotateRight()).Wall;

    private static bool MakesSquare(Cell cell) => 
        MakesSquare(cell, Vector2Int.up) || 
        MakesSquare(cell, Vector2Int.right) || 
        MakesSquare(cell, Vector2Int.down) || 
        MakesSquare(cell, Vector2Int.left);

    public static IEnumerable<Cell> AntiEdgesSquare(Cell cell) => cell.Neighbours().Where(MakesSquare).Union(Helpers.Diagonals(cell).Where(MakesSquare));

    public static bool Bad(Cell c) => MakesCross(c);

    private static Dictionary<Cell, float> cellPrices = new Dictionary<Cell, float>();

    private static float CalculateCellPriceRandom(Cell cell) {
        return Rand.Range(0, 1f);
    }

    public static float CellPrice(Cell cell, bool reroll = false) {
        if (!cellPrices.ContainsKey(cell) || reroll) {
            cellPrices[cell] = CalculateCellPriceRandom(cell);
        }
        return cellPrices[cell] + (Bad(cell) ? 1 : 0);
    }

    private static IEnumerable<Cell> BorderCells(IEnumerable<Cell> cells) {
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
            } else if (outer(current.Shift(direction))) {
                current = current.Shift(direction);
                result.Add(current);
            } else {
                direction = direction.RotateLeft();
            }
            if (current == start) {
                return result;
            }
        }
        Debug.LogError("Too many iterations for border!");
        return result;
    }

    private static Cell CheapestBorderCell(IEnumerable<Cell> cells) {
        var border = BorderCells(cells);
        Debug.LogFormat($"border: {border.ExtToString()}");
        Map<Cell, float> fixedPrices = new Map<Cell, float>();
        foreach (var c in border) {
            fixedPrices[c] = CellPrice(c);
        }
        return border.MinBy((a, b) => fixedPrices[a] < fixedPrices[b]);
    }

    public float speed = 1;
    public Task commandToContinue;

    private async Task CommandToContinue() {
        if (speed * Time.deltaTime > 1) {
            await Task.Delay(1);
        } else {
            await Task.Delay((int)(1000 / speed));
        }
    }

    private async Task GenerateBiome(Biome biome, bool pauses = false) {
        var boss = bossBiome == biome;
        Game.instance.mainWorld.currentBiome = biome;

        var start = Game.instance.mainWorld.GetCell(Vector2Int.zero);
        if (cellOrderList.Count > 0) {
            start = CheapestBorderCell(cellOrderList);
            Game.Debug($"Start of biome: {start}");
        }

        var cellOrder = Algorithm.PrimDynamic(
            start: start,
            edges: c => c.Neighbours().Where(c => c.Wall)
                .Select(c => new Weighted<Cell>(c, CellPrice(c, reroll: true))),
            maxSteps: 100000
        ).Take(biome.Size);

        var biomeCells = new List<Cell>();

        int i = 0;
        foreach (Cell c in cellOrder) {

            c.orderInBiome = i;
            c.Biome = biome;
            biomeCells.Add(c);

            AddFloorCell(c, biome);

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
        Debug.LogFormat($"Taken Cells Max Price: {cellOrderList.Max(c => CellPrice(c))}");
    }

    private bool MakeFloorCellIfCross(Cell from, Vector2Int direction) {
        if (MakesCross(from, direction)) {
            AddFloorCell(from.Shift(direction), from.Biome);
            return true;
        }
        return false;
    }

    public bool RemoveCrossesIteration() {
        Debug.LogFormat("RemoveCrossesIteration");
        bool changed = false;
        foreach (var cell in cellOrderList.ToList()) {
            foreach (var move in Helpers.Moves) {
                if (MakeFloorCellIfCross(cell, move)) {
                    changed = true;
                }
            }
        }
        Debug.LogFormat(changed ? "Found some crosses" : "No crosses found");
        return changed;
    }

    private void AddTreesNearForest() {
        if (cellOrderList.Where(cell => cell.Biome == Library.instance.darkrootForest).Count() == 0) {
            return;
        }
        foreach (var cell in BorderCells(cellOrderList.Where(cell => cell.Biome == Library.instance.darkrootForest))
            .Where(cell => cell.Wall)
        ) {
            if (Rand.rndEvent(1f)) {
                AddFloorCell(cell, Library.instance.darkrootForest);
                Game.GenerateFigure(cell, Library.instance.tree);
            }
        }
    }

    private void RemoveCrosses() {
        for (int i = 0; i < 3; i++) {
            if (!RemoveCrossesIteration()) return;
        }
        Debug.LogFormat("Not enough RemoveCrosses iterations");
    }

    private async Task Postprocess() {
        AddTreesNearForest();
        RemoveCrosses();
    }

    public async Task GenerateWorld() {
        Debug.LogFormat($"GenerateWorld, id = {this.GetInstanceID()}");

        cellOrderList = new List<Cell>();
        bossBiome = Library.instance.bossBiomes.Rnd();
        bossBiome = Library.instance.inferno;
        Game.instance.bossName = bossBiome.bossName;

        startBiome = Library.instance.inferno;

        biomesOrder = Library.instance.biomes.Shuffled();

        //foreach (var biome in biomesOrder) {
        //    await GenerateBiome(biome);
        //}

        await GenerateBiome(Library.instance.inferno);

        await Postprocess();

        var start = cellOrderList.FirstOrDefault(cell => cell.Biome == Library.instance.dungeon && cell.orderInBiome == 0);
        if (start == null) {
            start = cellOrderList.First(cell => cell.orderInBiome == 0);
        }
        Game.instance.player = Game.GenerateFigure(start, Game.instance.playerSample);

        foreach (var cell in cellOrderList) PopulateCell(cell);

        var bossGenerator = bossBiome.GetComponent<IBossGenerator>();
        if (bossGenerator != null) {
            bossGenerator.GenerateBoss(cellOrderList);
        }

        for (int i = 0; i < storeCount; i++) {
            GenerateStore();
        }

        for (int i = 0; i < Metagame.instance.FreeHealingPotionsCount; i++) {
            var cell = cellOrderList.Where(cell => cell.figures.Count() == 0).Rnd();
            if (cell != null) {
                Game.GenerateFigure(cell, Game.instance.healingPotionSample);
            }
        }

        var temporalGhost = Game.GenerateFigure(
            cellOrderList.Where(cell => cell.figures.Count() == 0).Rnd(),
            Library.Get<TemporalGhost>()
        );

        // fog
        foreach (var cell in cellOrderList) {
            cell.AfterStoresAdded();
        }
    }

    private async Task GenerateTestWorld() {
        AddFloorCell(Game.instance.mainWorld.GetCell(Vector2Int.zero));
        AddFloorCell(Game.instance.mainWorld.GetCell(new Vector2Int(0, 1)));
        AddFloorCell(Game.instance.mainWorld.GetCell(new Vector2Int(0, 2)));
        Game.instance.player = GenerateFigure(Game.instance.playerSample, 0, 0);
        GenerateFigure(Library.Get<BackArmor>(), 0, 1);
        GenerateFigure(Library.Get<Harpy>(), 0, 2);
    }

    public void Start() {
        speed = 10000;
    }

    public void AddFloorCell(Cell c, Biome biome = null) {
        biome ??= Library.instance.dungeon;

        if (!c.Wall) {
            Debug.LogFormat($"Attempt to make floor from floor: {c}");
            return;
        }
        //Debug.LogFormat($"Add floor cell {c}");
        cellOrderList.Add(c);
        c.order = cellOrderList.Count - 1;
        c.fieldCell.wall = false;
        c.gameObject.SetActive(true);
        c.Biome = biome;
        c.UpdateCell();

        c.Neighbours8().ForEach(c2 => {
            if (c2.fieldCell.wall) {
                c2.Biome = c.Biome;
                c2.UpdateCell();
                c2.gameObject.SetActive(true);
            }
        });
    }

    public void PopulateCell(Cell cell) {
        if (cell.GetFigure<Figure>() != null) {
            return;
        } else if (cell.orderInBiome == 1 && cell.Biome == startBiome && !Game.instance.Metagame.HasAscention<NoStartingWeapon>()) {
            Game.GenerateFigure(cell, Game.instance.weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().startingWeight));
            return;
        } else if (cell.Biome == startBiome && Game.instance.startingItemsSamples.Count() > 0) {
            Game.GenerateFigure(cell, Game.instance.startingItemsSamples.First());
            Game.instance.startingItemsSamples.RemoveAt(0);
            return;
        } else if (cell.Biome == Library.instance.crypt && cell.orderInBiome == Library.instance.crypt.Size - 1 && bossBiome == Library.instance.crypt) {
            Game.GenerateFigure(cell, Game.instance.lichSample);
            return;
        }

        if (cell.Biome == Library.instance.darkrootForest && Rand.rndEvent(0.1f)) {
            Game.GenerateFigure(cell, Library.instance.tree);
        } else if ((Game.instance.player.figure.Location.position - cell.position).magnitude > 6 && false && Rand.rndEvent(Metagame.instance.MonsterProbability)) {
            Game.GenerateFigure(cell, cell.Biome.monsterSamples.Concat(cell.Biome.additionalMonsterSamples).ToList().weightedRnd());
        } else if (Rand.rndEvent(0.004f)) {
            Game.GenerateFigure(cell, Game.instance.weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().fieldWeight));
        } else if (Rand.rndEvent(0)) {
            Game.GenerateFigure(cell, Game.instance.itemSamples.rnd());
        } else if (Rand.rndEvent(0.3f)) {
            Game.GenerateFigure(cell, cell.Biome.terrainSamples.weightedRnd());
        }
    }

    public static T GenerateFigure<T>(T sample, int x, int y, Board board = null) where T : MonoBehaviour {
        board ??= Game.instance.mainWorld;
        return Game.GenerateFigure(board.GetCell(new Vector2Int(x, y)), sample);
    }

    public void RestockAllStores() {
        Game.instance.stores.ForEach(RestockStore);
    }

    private void Sell(Cell location, MonoBehaviour sample, int price = -1) {
        var item = Game.GenerateFigure(location, sample);
        if (price == -1) {
            var rules = item.GetComponent<ItemGenerationRules>();
            price = rules != null ? Rand.rnd(rules.minPrice, rules.maxPrice) : 0;
        }
        Game.GenerateFigure(location, Game.instance.paidCellSample).SetPrice((int)Math.Ceiling(price * Metagame.instance.PricesMultiplier));
        var info = Game.GenerateFigure(location.Shift(Vector2Int.down), Game.instance.infoSample);
        info.explainable = item.GetComponent<IExplainable>();
    }

    private void GenerateStore() {
        var newStore = Instantiate(Game.instance.boardSample, transform);
        newStore.currentBiome = Library.instance.dungeon;
        newStore.GetCell(Vector2Int.zero).Vicinity(storeRadius + 1).ForEach(cell => cell.gameObject.SetActive(true));
        newStore.GetCell(Vector2Int.zero).Vicinity(storeRadius).ForEach(cell => {
            cell.fieldCell.wall = false;
            cell.UpdateCell();
        });
        newStore.gameObject.SetActive(false);
        Game.instance.stores.Add(newStore);

        var entry = Game.GenerateFigure(
            Game.instance.mainWorld.cells.Where(c => c.figures.Count() == 0 && !c.Wall).Rnd(), 
            Game.instance.portalSample
        );
        var exit = Game.GenerateFigure(newStore.GetCell(Vector2Int.zero), Game.instance.portalSample);
        entry.second = exit;
        exit.second = entry;

        Sell(newStore[-2, 3], Game.instance.healingPotionSample);
        Sell(newStore[0, 3], Game.instance.weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().storeWeight));
        Sell(newStore[2, 3], Game.instance.ringOfTerraformingSample);

        Sell(newStore[-2, -2], Game.instance.itemSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().storeWeight));
        Sell(newStore[0, -2], Game.instance.itemSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().storeWeight));
        Sell(newStore[2, -2], Game.instance.itemSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().storeWeight));
    }

    private void RestockStore(Board store) {
        var ringOfTerraformingPlace = store[2, 3];
        if (ringOfTerraformingPlace.figures.Count == 0) {
            Sell(ringOfTerraformingPlace, Game.instance.ringOfTerraformingSample);
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            RemoveCrossesIteration();
        }
    }
}
