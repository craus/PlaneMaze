using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public SparseCollections.Sparse2DChunkBasedMatrix<Cell> map = new SparseCollections.Sparse2DChunkBasedMatrix<Cell>();

    public Field field;
    public Cell cellSample;
    public Transform cellParent;
    public Transform figureParent;

    public List<Cell> cells;

    public List<IMovable> movables;

    public bool silentMode = false;

    public Biome currentBiome;

    public Cell GetCell(int x, int y) {
        ShowCell(x, y);
        return map[x, y];
    }

    public Cell GetCell(Vector2Int position) => GetCell(position.x, position.y);

    public Cell this[int x, int y] => GetCell(new Vector2Int(x, y));

    public void Awake() {
        movables = new List<IMovable>();
        new ValueTracker<List<IMovable>>(() => movables.ToList(), v => movables = v.ToList());
        new ValueTracker<bool>(() => gameObject.activeSelf, v => gameObject.SetActive(v));
    }

    private Cell GenerateCell(int x, int y) {
        var cell = Instantiate(cellSample);
        cells.Add(cell);
        cell.gameObject.SetActive(false);
        cell.position = new Vector2Int(x, y);
        //Debug.LogFormat($"Generate cell ({x}, {y})");
        cell.Biome = currentBiome;
        cell.gameObject.name = $"Cell ({x}, {y})";
        field[x, y].wall = true;
        cell.SetFieldCell(field[x, y]);
        cell.transform.SetParent(cellParent);
        cell.transform.position = new Vector3(x, y, 0);
        cell.board = this;

        return cell;
    }

    private void ShowCell(int x, int y) {
        if (map[x, y] == null) {
            map[x, y] = GenerateCell(x, y);
        }
    }
}
