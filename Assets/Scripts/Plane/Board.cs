using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public SparseCollections.Sparse2DChunkBasedMatrix<Cell> map = new SparseCollections.Sparse2DChunkBasedMatrix<Cell>();

    public Field field;
    public Cell cellSample;
    public Figure cracksSample;
    public Teleport teleportSample;
    public Transform cellParent;
    public Transform figureParent;

    public List<Cell> cells;

    public bool silentMode = false;

    public Biome currentBiome;

    public Cell GetCell(Vector2Int position) {
        ShowCell(position.x, position.y);
        return map[position.x, position.y];
    }

    public Cell this[int x, int y] => GetCell(new Vector2Int(x, y));

    public void Awake() {
        new ValueTracker<bool>(() => gameObject.activeSelf, v => gameObject.SetActive(v));
    }

    private void Start() {
        //for (int i = 0; i < 10; i++) {
        //    for (int j = 0; j < 10; j++) {
        //        ShowCell(i, j);
        //    }
        //}
    }

    private void Update() {
        int xMin = (int)Mathf.Round(Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect);
        int xMax = (int)Mathf.Round(Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect);
        int yMin = (int)Mathf.Round(Camera.main.transform.position.y - Camera.main.orthographicSize);
        int yMax = (int)Mathf.Round(Camera.main.transform.position.y + Camera.main.orthographicSize);

        for (int i = xMin; i <= xMax; i++) {
            for (int j = yMin; j <= yMax; j++) {
                //ShowCell(i, j);
            }
        }
    }

    private void GenerateFigure(Cell cell, MonoBehaviour sample) {
        var f = Instantiate(sample);
        f.GetComponent<Figure>().Move(cell);
    }

    private Cell GenerateCell(int x, int y) {
        var cell = Instantiate(cellSample);
        cells.Add(cell);
        if (silentMode) {
            cell.gameObject.SetActive(false);
        }
        cell.position = new Vector2Int(x, y);
        cell.biome = currentBiome;
        cell.UpdateBiome();
        cell.gameObject.name = $"Cell ({x}, {y})";
        field[x, y].wall = true;
        cell.SetFieldCell(field[x, y]);
        cell.transform.SetParent(cellParent);
        cell.transform.position = new Vector3(x, y, 0);
        cell.board = this;

        if (cell.fieldCell.teleport) {
            GenerateFigure(cell, teleportSample);
        }

        if (!cell.fieldCell.wall && !cell.fieldCell.teleport) {
            if (Rand.rndEvent(0.15f)) {
                GenerateFigure(cell, cracksSample);
            }
        }

        return cell;
    }

    private void ShowCell(int x, int y) {
        if (map[x, y] == null) {
            map[x, y] = GenerateCell(x, y);
        }
    }
}
