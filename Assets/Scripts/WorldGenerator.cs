using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class WorldGenerator : Singletone<WorldGenerator>
{
    public int storeCount = 4;
    public int storeRadius = 5;

    public List<Cell> cellOrderList;
    public Biome bossBiome;
    public List<Biome> biomesOrder;

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
        UnityEngine.Debug.LogError("Too many iterations for border!");
        return result;
    }

    private static Cell CheapestBorderCell(IEnumerable<Cell> cells) {
        var border = BorderCells(cells);
        UnityEngine.Debug.LogFormat($"border: {border.ExtToString()}");
        Map<Cell, float> fixedPrices = new Map<Cell, float>();
        foreach (var c in border) {
            fixedPrices[c] = WorldGenerator.CellPrice(c);
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
        //commandToContinue = new Task(() => { });
        //await commandToContinue;
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

    public async Task GenerateWorld() {
        cellOrderList = new List<Cell>();
        bossBiome = Library.instance.bossBiomes.Rnd();

        biomesOrder = Library.instance.biomes.Shuffled();

        foreach (var biome in biomesOrder) {
            await GenerateBiome(biome);
        }

        var start = cellOrderList.First(cell => cell.biome == Library.instance.dungeon && cell.orderInBiome == 0);
        Game.instance.player = Game.GenerateFigure(start, Game.instance.playerSample);

        foreach (var cell in cellOrderList) PopulateCell(cell);

        if (bossBiome == Library.instance.darkrootForest) {
            var witch = Game.GenerateFigure(
                cellOrderList.Where(cell => cell.biome == Library.instance.darkrootForest && cell.figures.Count() == 0).Rnd(),
                Library.instance.darkrootForest.GetComponent<DarkrootForest>().witch);
            var sister = Game.GenerateFigure(
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
            Game.GenerateFigure(cell, Library.instance.tree);
        }

        for (int i = 0; i < storeCount; i++) {
            GenerateStore();
        }

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

    public void AddFloorCell(Cell c) {
        cellOrderList.Add(c);
        c.order = cellOrderList.Count - 1;
        c.fieldCell.wall = false;
        c.UpdateCell();
    }

    public void PopulateCell(Cell cell) {
        if (cell.orderInBiome == 1 && cell.biome == Library.instance.dungeon && !Game.instance.Metagame.HasAscention<NoStartingWeapon>()) {
            Game.GenerateFigure(cell, Game.instance.weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().startingWeight));
            return;
        } else if (cell.biome == Library.instance.dungeon && Game.instance.startingItemsSamples.Count() > 0) {
            Game.GenerateFigure(cell, Game.instance.startingItemsSamples.First());
            Game.instance.startingItemsSamples.RemoveAt(0);
            return;
        } else if (cell.biome == Library.instance.crypt && cell.orderInBiome == Library.instance.crypt.Size - 1 && bossBiome == Library.instance.crypt) {
            Game.GenerateFigure(cell, Game.instance.lichSample);
            return;
        }

        if (cell.biome == Library.instance.darkrootForest && Rand.rndEvent(0.1f)) {
            Game.GenerateFigure(cell, Library.instance.tree);
        } else if ((Game.instance.player.figure.Location.position - cell.position).magnitude > 6 && Rand.rndEvent(Metagame.instance.MonsterProbability)) {
            Game.GenerateFigure(cell, cell.biome.monsterSamples.weightedRnd());
        } else if (Rand.rndEvent(0.004f)) {
            Game.GenerateFigure(cell, Game.instance.weaponSamples.rnd(weight: w => w.GetComponent<ItemGenerationRules>().fieldWeight));
        } else if (Rand.rndEvent(Metagame.instance.HealingPotionSpawnProbability)) {
            Game.GenerateFigure(cell, Game.instance.healingPotionSample);
        } else if (Rand.rndEvent(0)) {
            Game.GenerateFigure(cell, Game.instance.itemSamples.rnd());
        } else if (Rand.rndEvent(0.3f)) {
            Game.GenerateFigure(cell, cell.biome.terrainSamples.weightedRnd());
        }
    }

    public static T GenerateFigure<T>(T sample, int x, int y, Board board = null) where T : MonoBehaviour {
        board ??= Game.instance.mainWorld;
        return Game.GenerateFigure(board.GetCell(new Vector2Int(x, y)), sample);
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
        newStore.silentMode = true;
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
}
