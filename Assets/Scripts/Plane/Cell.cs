using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public SpriteRenderer sprite;
    public TMPro.TextMeshProUGUI orderText;
    public TMPro.TextMeshProUGUI priceText;

    [SerializeField] private Biome biome;
    public Biome Biome {
        get => biome;
        set {
            if (biome == value) return;
            biome = value;
            UpdateBiome();
        }
    }

    public Color lockedColor;
    public Color darknessColor;
    public GameObject wallModel;
    public GameObject floorModel;

    public Vector2Int position;
    public int order = -1;
    public int orderInBiome = -1;

    public FieldCell fieldCell;
    public Board board;

    [SerializeField] private bool dark = false;
    public bool Dark
    {
        get => dark;
        set
        {
            dark = value;
            UpdateCell();
        }
    }

    public bool Wall
    {
        get => fieldCell.wall;
        set {
            fieldCell.wall = value;
            UpdateCell();
        }
    }

    public bool Ordered => order != -1;

    public bool Locked => order >= Game.instance.unlockedCells;

    [SerializeField] private List<Figure> debugFiguresList = new List<Figure>();
    public HashSet<Figure> figures = new HashSet<Figure>();

    [ContextMenu("UpdateDebugInfo")]
    public void UpdateDebugInfo() {
        debugFiguresList = figures.ToList();
    }

    public void Awake() {
        new ValueTracker<List<Figure>>(() => figures.ToList(), v => figures = new HashSet<Figure>(v), defaultValue: new List<Figure>());
    }

    public Color Color() {
        //return Game.instance.GetCellColor(position);

        if (Wall) return biome.wallModel.GetComponentInChildren<SpriteRenderer>().color;
        if (Locked) return lockedColor;
        if (dark) return darknessColor;
        return biome.floorModel.GetComponentInChildren<SpriteRenderer>().color;
    }

    public void UpdateBiome() {
        Destroy(floorModel);
        Destroy(wallModel);
        floorModel = Instantiate(biome.floorModel, transform);
        wallModel = Instantiate(biome.wallModel, transform);
        foreach (var tileModel in GetComponentsInChildren<TileModel>()) {
            tileModel.SetOffset(position);
        }
    }

    public void UpdateCell() {
        wallModel.SetActive(Wall);
        floorModel.SetActive(!Wall);
        if (orderText != null) {
            orderText.text = order.ToString();
            orderText.gameObject.SetActive(!Wall);
        }
        if (priceText != null) {
            priceText.gameObject.SetActive(!Wall);
            priceText.text = WorldGenerator.CellPrice(this).ToString("0.000");
        }
    }

    public void SetFieldCell(FieldCell fieldCell) {
        this.fieldCell = fieldCell;
        UpdateCell();
    }

    public Cell Shift(int dx, int dy) => Shift(new Vector2Int(dx, dy));

    public Cell Shift(Vector2Int delta) {
        return board.GetCell(position + delta);
    }

    public IEnumerable<Cell> Neighbours() {
        yield return Shift(Vector2Int.up);
        yield return Shift(Vector2Int.down);
        yield return Shift(Vector2Int.left);
        yield return Shift(Vector2Int.right);
    }

    public IEnumerable<Cell> Neighbours8() {
        yield return Shift(Vector2Int.up);
        yield return Shift(Vector2Int.down);
        yield return Shift(Vector2Int.left);
        yield return Shift(Vector2Int.right);
        yield return Shift(new Vector2Int(1, 1));
        yield return Shift(new Vector2Int(1, -1));
        yield return Shift(new Vector2Int(-1, 1));
        yield return Shift(new Vector2Int(-1, -1));
    }

    public IEnumerable<Cell> Vicinity(int radius) => Vicinity(radius, radius);

    internal void OnOccupyingUnitAttacked(Unit target) {
        foreach (var f in GetFigures<IOnOccupyingUnitAttackedListener>()) {
            f.OnOccupyingUnitAttacked(target);
        }
    }

    public IEnumerable<Cell> Vicinity(int maxDx, int maxDy) {
        for (int i = -maxDx; i <= maxDx; i++) {
            for (int j = -maxDy; j <= maxDy; j++) {
                yield return Shift(i, j);
            }
        }
    }

    internal IEnumerable<Cell> SmallestVicinity(Func<IEnumerable<Cell>, bool> criteria) {
        for (int i = 1; i < 10; i++) {
            var v = Vicinity(i);
            if (criteria(v)) {
                return v;
            }
        }
        return null;
    }

    public void AfterStoresAdded() {
        if (biome.GetComponent<DarkrootForest>()) {
            Game.GenerateFigure(this, biome.GetComponent<DarkrootForest>().fog);
        }
    }

    public T GetFigure<T>() => figures.Select(f => f.GetComponent<T>()).FirstOrDefault(t => t != null);
    public IEnumerable<T> GetFigures<T>() => figures.Select(f => f.GetComponent<T>()).Where(t => t != null);
    public T GetFigure<T>(Func<T, bool> criteria) => figures.Select(f => f.GetComponent<T>()).FirstOrDefault(t => t != null && criteria(t));
    public IEnumerable<T> GetFigures<T>(Func<T, bool> criteria) => figures.Select(f => f.GetComponent<T>()).Where(t => t != null && criteria(t));

    public bool Free => !Wall && !Locked && !figures.Any(f => f.GetComponent<Unit>() != null && f.GetComponent<Unit>().OccupiesPlace);
    public bool FreeAndNoWolfTrap => Free && GetFigure<WolfTrap>() == null;

    public override string ToString() => position.ToString();
}
